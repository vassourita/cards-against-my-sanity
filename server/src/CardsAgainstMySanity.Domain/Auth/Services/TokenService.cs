using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.SharedKernel;
using Microsoft.IdentityModel.Tokens;

namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public class TokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly TokenSettings _tokenSettings;

        public TokenService(TokenSettings tokenSettings, IRefreshTokenRepository refreshTokenRepository)
        {
            _tokenSettings = tokenSettings;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public Claim[] GetClaims(IUser user)
            => new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

        public ClaimsPrincipal GeneratePrincipal(IUser user)
        {
            var claims = GetClaims(user);
            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }

        public string GenerateAccessToken(IUser user)
        {
            var now = DateTime.UtcNow;
            var claims = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));
            var creds = new SigningCredentials(key, _tokenSettings.SecurityAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _tokenSettings.AccessTokenIssuer,
                audience: _tokenSettings.AccessTokenAudience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_tokenSettings.AccessTokenExpirationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(bool isGuest)
        {
            var expiration = DateTime.UtcNow.AddMinutes(
                isGuest ? _tokenSettings.GuestRefreshTokenExpirationInMinutes : _tokenSettings.UserAccountRefreshTokenExpirationInMinutes
            );
            var now = DateTime.UtcNow;
            var refreshToken = new RefreshToken(Guid.NewGuid().ToString(), expiration);
            return refreshToken;
        }

        public Result<string> Refresh(RefreshToken refreshToken, IUser user)
        {
            if (!IsRefreshTokenValid(refreshToken))
            {
                return Result<string>.Fail();
            }
            var token = GenerateAccessToken(user);
            return Result<string>.Ok(token);
        }

        public enum ValidationError
        {
            AccessTokenExpired,
            AccessTokenInvalid,
        }

        public Result<ClaimsPrincipal, ValidationError> IsAccessTokenValid(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new()
            {
                ValidateAudience = true,
                ValidAudience = _tokenSettings.AccessTokenAudience,
                ValidateIssuer = true,
                ValidIssuer = _tokenSettings.AccessTokenIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
            };

            SecurityToken validatedToken;
            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out validatedToken);
                if (validatedToken.ValidTo < DateTime.UtcNow || validatedToken.ValidFrom > DateTime.UtcNow)
                {
                    return Result<ClaimsPrincipal, ValidationError>.Fail(ValidationError.AccessTokenExpired);
                }
                return Result<ClaimsPrincipal, ValidationError>.Ok(principal);
            }
            catch (Exception)
            {
                return Result<ClaimsPrincipal, ValidationError>.Fail(ValidationError.AccessTokenInvalid);
            }
        }

        private bool IsRefreshTokenValid(RefreshToken refreshToken)
        {
            if (refreshToken == null || refreshToken.Expired)
            {
                return false;
            }
            return true;
        }
    }
}