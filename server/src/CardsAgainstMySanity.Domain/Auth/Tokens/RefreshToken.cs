namespace CardsAgainstMySanity.Domain.Auth.Tokens
{
    public class RefreshToken
    {
        public Guid Token { get; protected set; }
        public DateTime ExpiresAt { get; protected set; }
        public bool Expired { get => ExpiresAt < DateTime.UtcNow; }
        public DateTime CreatedAt { get; protected set; }

        public Guid UserId { get; protected set; }

        // For EF
        protected RefreshToken()
        { }

        public RefreshToken(string token, DateTime expiresAt)
        {
            Token = Guid.Parse(token);
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
        }
    }
}