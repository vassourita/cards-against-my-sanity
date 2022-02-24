namespace CardsAgainstMySanity.Domain.Auth.Tokens;

using CardsAgainstMySanity.Domain.Providers;

public class RefreshToken
{
    public Guid Token { get; protected set; }
    public DateTime ExpiresAt { get; protected set; }
    public bool Expired => this.ExpiresAt < DateTime.UtcNow;
    public DateTime CreatedAt { get; protected set; }

    public Guid UserId { get; protected set; }

    // For EF
    protected RefreshToken()
    { }

    public RefreshToken(Guid token, DateTime expiresAt, IDateTimeProvider dateTimeProvider)
    {
        this.Token = token;
        this.ExpiresAt = expiresAt;
        this.CreatedAt = dateTimeProvider.UtcNow;
    }
}
