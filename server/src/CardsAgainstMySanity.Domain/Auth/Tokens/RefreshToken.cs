namespace CardsAgainstMySanity.Domain.Auth.Tokens
{
    public class RefreshToken
    {
        public Guid Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool Expired { get => ExpiresAt < DateTime.UtcNow; }
        public DateTime CreatedAt { get; private set; }

        public RefreshToken(string token, DateTime expiresAt)
        {
            Token = Guid.Parse(token);
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
        }
    }
}