namespace CardsAgainstMySanity.Domain.Identity.Tokens;

public class Token
{
    public Token(string tokenValue, DateTimeOffset expiresAt, string type)
    {
        TokenValue = tokenValue;
        ExpiresAt = expiresAt;
        Type = type;
    }

    public string TokenValue { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public string Type { get; private set; }
}
