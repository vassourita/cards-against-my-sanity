using System.Collections.ObjectModel;

namespace CardsAgainstMySanity.Domain.Auth
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; protected set; }
        public string IpAddress { get; protected set; }
        public string AccessToken { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public ICollection<RefreshToken> RefreshTokens { get; protected set; }

        public User(string username)
        {
            Id = Guid.NewGuid();
            Username = username;
            CreatedAt = DateTime.UtcNow;
            RefreshTokens = new Collection<RefreshToken>();
        }

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