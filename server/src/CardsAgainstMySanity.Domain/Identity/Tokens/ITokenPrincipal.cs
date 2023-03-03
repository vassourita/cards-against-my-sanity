namespace CardsAgainstMySanity.Domain.Identity.Tokens;

public interface ITokenPrincipal
{
    Guid Id { get; }
    string Name { get; }
}