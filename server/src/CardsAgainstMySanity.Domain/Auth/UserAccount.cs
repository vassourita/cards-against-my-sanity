using System.Collections.ObjectModel;
using CardsAgainstMySanity.Domain.Auth.Tokens;

namespace CardsAgainstMySanity.Domain.Auth
{
    public class UserAccount : IUser
    {
        // For EF
        protected UserAccount()
        {
            RefreshTokens = new Collection<RefreshToken>();
        }

        public UserAccount(string username, string ipAddress)
        {
            Id = Guid.NewGuid();
            Username = username;
            IpAddress = ipAddress;
            CreatedAt = DateTime.UtcNow;
            RefreshTokens = new Collection<RefreshToken>();
        }

        public Guid Id { get; protected set; }
        public string AvatarUrl { get; protected set; }
        public string Username { get; protected set; }
        public string Email { get; protected set; }
        public string PasswordHash { get; protected set; }
        public string IpAddress { get; protected set; }
        public string AccessToken { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public DateTime LastPong { get; protected set; }

        public ICollection<RefreshToken> RefreshTokens { get; protected set; }

        public void SetAccessToken(string accessToken, string ipAddress)
        {
            AccessToken = accessToken;
            IpAddress = ipAddress;
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }
    }
}