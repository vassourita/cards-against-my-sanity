namespace CardsAgainstMySanity.Domain.Identity.Handlers.Commands;

using System.Threading;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Identity.Requests;
using CardsAgainstMySanity.Domain.Identity.Results;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using MediatR;

public class ValidateTokenHandler : IRequestHandler<ValidateTokenRequest, TokenValidationResult>
{
    private readonly TokenValidator _tokenValidator;

    public ValidateTokenHandler(TokenValidator tokenValidator) => _tokenValidator = tokenValidator;

    public async Task<TokenValidationResult> Handle(ValidateTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _tokenValidator.ValidateAsync(request.Token);

        if (user == null)
            return TokenValidationResult.Fail();

        return TokenValidationResult.Ok(user);
    }
}