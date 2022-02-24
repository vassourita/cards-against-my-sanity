namespace CardsAgainstMySanity.Domain.Auth.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenSettings _tokenSettings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenService(TokenSettings tokenSettings, IRefreshTokenRepository refreshTokenRepository, IDateTimeProvider dateTimeProvider)
    {
        this._tokenSettings = tokenSettings;
        this._refreshTokenRepository = refreshTokenRepository;
        this._dateTimeProvider = dateTimeProvider;
    }

    // for moq
    protected TokenService() { }

    public static Claim[] GetClaims(IUser user)
        => new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
        };

    public static ClaimsPrincipal GeneratePrincipal(IUser user)
    {
        var claims = GetClaims(user);
        var identity = new ClaimsIdentity(claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }

    public string GenerateAccessToken(IUser user)
    {
        var now = this._dateTimeProvider.UtcNow;
        var claims = GetClaims(user);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._tokenSettings.SecretKey));
        var creds = new SigningCredentials(key, this._tokenSettings.SecurityAlgorithm);

        var token = new JwtSecurityToken(
            issuer: this._tokenSettings.AccessTokenIssuer,
            audience: this._tokenSettings.AccessTokenAudience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(this._tokenSettings.AccessTokenExpirationInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(bool isGuest)
    {
        var now = this._dateTimeProvider.UtcNow;
        var expiration = now.AddMinutes(
            isGuest ? this._tokenSettings.GuestRefreshTokenExpirationInMinutes : this._tokenSettings.UserAccountRefreshTokenExpirationInMinutes
        );
        var refreshToken = new RefreshToken(Guid.NewGuid(), expiration, this._dateTimeProvider);
        return refreshToken;
    }

    public Result<string> Refresh(RefreshToken refreshToken, IUser user, string ipAddress)
    {
        if (!IsRefreshTokenValid(refreshToken))
        {
            return Result<string>.Fail();
        }
        var token = this.GenerateAccessToken(user);
        user.SetAccessToken(token, ipAddress);
        return Result<string>.Ok(token);
    }

    public enum ValidationError
    {
        AccessTokenExpired,
        AccessTokenInvalid,
    }

    public virtual ClaimsPrincipal ValidateJWT(string token, TokenValidationParameters validationParameters)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        if (validatedToken.ValidTo < this._dateTimeProvider.UtcNow || validatedToken.ValidFrom > this._dateTimeProvider.UtcNow)
        {
            throw new SecurityTokenExpiredException();
        }
        return principal;
    }

    public virtual Result<ClaimsPrincipal, ValidationError> IsAccessTokenValid(string accessToken)
    {
        TokenValidationParameters validationParameters = new() // shouldn't be put here, I guess
        {
            ValidateAudience = true,
            ValidAudience = this._tokenSettings.AccessTokenAudience,
            ValidateIssuer = true,
            ValidIssuer = this._tokenSettings.AccessTokenIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._tokenSettings.SecretKey)),
        };
        try
        {
            var principal = this.ValidateJWT(accessToken, validationParameters);
            return Result<ClaimsPrincipal, ValidationError>.Ok(principal);
        }
        catch (Exception)
        {
            return Result<ClaimsPrincipal, ValidationError>.Fail(ValidationError.AccessTokenInvalid);
        }
    }

    public static bool IsRefreshTokenValid(RefreshToken refreshToken)
    {
        if (refreshToken == null || refreshToken.Expired)
        {
            return false;
        }
        return true;
    }
}
