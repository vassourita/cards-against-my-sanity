namespace CardsAgainstMySanity.Domain.Auth;

using System.Collections.ObjectModel;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;

public class Guest : IUser
{
    // For EF
    protected Guest() => this.RefreshTokens = new Collection<RefreshToken>();

    public Guest(string username, string ipAddress, string avatarUrl, IDateTimeProvider dateTimeProvider)
    {
        this.Id = Guid.NewGuid();
        this.Username = username;
        this.IpAddress = ipAddress;
        this.AvatarUrl = avatarUrl;
        this.CreatedAt = dateTimeProvider.UtcNow;
        this.RefreshTokens = new Collection<RefreshToken>();
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
        this.AccessToken = accessToken;
        this.IpAddress = ipAddress;
    }

    public void AddRefreshToken(RefreshToken refreshToken) => this.RefreshTokens.Add(refreshToken);
}
