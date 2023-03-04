namespace CardsAgainstMySanity.Domain.Identity.Tokens;

public class JwtConfiguration
{
    public string SigningKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpiresInMinutes { get; set; }
    public int RefreshTokenExpiresInHours { get; set; }
}
