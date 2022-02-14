using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public class AccessService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly TokenSettings _tokenSettings;

        public AccessService(IGuestRepository guestRepository, TokenSettings tokenSettings)
        {
            _guestRepository = guestRepository;
            _tokenSettings = tokenSettings;
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

        public bool IsRefreshTokenValid(string refreshToken)
        {
            return true;
        }

        public string RefreshAccessToken(string refreshToken)
        {
            return "";
        }
        public bool IsUserAFK(string accessToken)
        {
            return true;
        }
    }
}