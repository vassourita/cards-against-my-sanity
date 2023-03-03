namespace CardsAgainstMySanity.Domain.Identity.Results;

using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.SharedKernel;

public class TokenValidationResult : Result<ITokenPrincipal>
{
    private TokenValidationResult(ITokenPrincipal user) : base(user)
    { }
    private TokenValidationResult() : base()
    { }

    public static TokenValidationResult Ok(ITokenPrincipal user) => new(user);
    public static TokenValidationResult Fail() => new();
}