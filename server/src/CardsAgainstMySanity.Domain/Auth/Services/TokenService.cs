using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;
using Microsoft.IdentityModel.Tokens;

namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public class TokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly TokenSettings _tokenSettings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(TokenSettings tokenSettings, IRefreshTokenRepository refreshTokenRepository, IDateTimeProvider dateTimeProvider)
        {
            _tokenSettings = tokenSettings;
            _refreshTokenRepository = refreshTokenRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        // for moq
        protected TokenService() { }

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
            var now = _dateTimeProvider.UtcNow;
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
            var now = _dateTimeProvider.UtcNow;
            var expiration = now.AddMinutes(
                isGuest ? _tokenSettings.GuestRefreshTokenExpirationInMinutes : _tokenSettings.UserAccountRefreshTokenExpirationInMinutes
            );
            var refreshToken = new RefreshToken(Guid.NewGuid(), expiration, _dateTimeProvider);
            return refreshToken;
        }

        public Result<string> Refresh(RefreshToken refreshToken, IUser user, string ipAddress)
        {
            if (!IsRefreshTokenValid(refreshToken))
            {
                return Result<string>.Fail();
            }
            var token = GenerateAccessToken(user);
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
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            if (validatedToken.ValidTo < _dateTimeProvider.UtcNow || validatedToken.ValidFrom > _dateTimeProvider.UtcNow)
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
                ValidAudience = _tokenSettings.AccessTokenAudience,
                ValidateIssuer = true,
                ValidIssuer = _tokenSettings.AccessTokenIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
            };
            try
            {
                var principal = ValidateJWT(accessToken, validationParameters);
                return Result<ClaimsPrincipal, ValidationError>.Ok(principal);
            }
            catch (Exception)
            {
                return Result<ClaimsPrincipal, ValidationError>.Fail(ValidationError.AccessTokenInvalid);
            }
        }

        public bool IsRefreshTokenValid(RefreshToken refreshToken)
        {
            if (refreshToken == null || refreshToken.Expired)
            {
                return false;
            }
            return true;
        }
    }
}