namespace CardsAgainstMySanity.Domain.Identity;

using CardsAgainstMySanity.Domain.Identity.Tokens;

public class Guest : ITokenPrincipal
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Guest(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
