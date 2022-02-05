using Microsoft.IdentityModel.Tokens;

namespace CardsAgainstMySanity.Domain.Auth.Tokens
{
    public class TokenSettings
    {
        public string SecretKey { get; set; }
        public int AccessTokenExpirationInMinutes { get; set; }
        public int GuestRefreshTokenExpirationInMinutes { get; set; }
        public int UserAccountRefreshTokenExpirationInMinutes { get; set; }
        public string AccessTokenIssuer { get; set; }
        public string AccessTokenAudience { get; set; }
        public string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;
    }
}