using System.Collections.ObjectModel;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;

namespace CardsAgainstMySanity.Domain.Auth
{
    public class Guest : IUser
    {
        // For EF
        protected Guest()
        {
            RefreshTokens = new Collection<RefreshToken>();
        }

        public Guest(string username, string ipAddress, string avatarUrl, IDateTimeProvider dateTimeProvider)
        {
            Id = Guid.NewGuid();
            Username = username;
            IpAddress = ipAddress;
            AvatarUrl = avatarUrl;
            CreatedAt = dateTimeProvider.UtcNow;
            RefreshTokens = new Collection<RefreshToken>();
        }

        public Guid Id { get; protected set; }
        public string AvatarUrl { get; protected set; }
        public string Username { get; protected set; }
        public string IpAddress { get; protected set; }
        public string AccessToken { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
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