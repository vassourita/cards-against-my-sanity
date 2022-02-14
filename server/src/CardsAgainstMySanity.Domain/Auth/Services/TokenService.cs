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

        public string GenerateAccessToken(IUser user)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));
            var creds = new SigningCredentials(key, _tokenSettings.SecurityAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _tokenSettings.AccessTokenIssuer,
                audience: _tokenSettings.AccessTokenAudience,
                claims: claims,
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
            if (!IsRefreshTokenValid(refreshToken, user))
            {
                return Result<string>.Fail();
            }
            var token = GenerateAccessToken(user);
            return Result<string>.Ok(token);
        }

        public bool IsAccessTokenValid(string accessToken, out ClaimsPrincipal principal)
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
                ValidateLifetime = true,
            };

            SecurityToken validatedToken;
            principal = tokenHandler.ValidateToken(accessToken, validationParameters, out validatedToken);
            if (principal == null)
            {
                return false;
            }
            return true;
        }

        private bool IsRefreshTokenValid(RefreshToken refreshToken, IUser user)
        {
            if (refreshToken == null
                || user == null
                || refreshToken.Expired
                || !user.RefreshTokens.Any(t => t.Token == refreshToken.Token))
            {
                return false;
            }
            return true;
        }
    }
}