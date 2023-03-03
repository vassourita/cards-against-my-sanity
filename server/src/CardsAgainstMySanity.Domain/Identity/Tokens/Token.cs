namespace CardsAgainstMySanity.Domain.Identity.Tokens;

public class Token
{
    public Token(string tokenValue, long expiresIn, string type)
    {
        TokenValue = tokenValue;
        ExpiresIn = expiresIn;
        Type = type;
    }

    public string TokenValue { get; private set; }

    public long ExpiresIn { get; private set; }

    public string Type { get; private set; }
}
