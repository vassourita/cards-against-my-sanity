using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public class TokenService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly TokenSettings _tokenSettings;

        public TokenService(TokenSettings tokenSettings, IGuestRepository guestRepository)
        {
            _tokenSettings = tokenSettings;
            _guestRepository = guestRepository;
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

        public async Task<string> Refresh(RefreshToken refreshToken)
        {
            if (refreshToken.Expired)
            {
                return null;
            }
            var user = await _guestRepository.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return null;
            }
            return GenerateAccessToken(user);
        }
    }
}