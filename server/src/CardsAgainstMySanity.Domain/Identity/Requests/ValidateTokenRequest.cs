namespace CardsAgainstMySanity.Domain.Identity.Requests;

using CardsAgainstMySanity.Domain.Identity.Results;
using MediatR;

public record ValidateTokenRequest(string Token) : IRequest<TokenValidationResult>
{
}
