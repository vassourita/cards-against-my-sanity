namespace CardsAgainstMySanity.Domain.Identity.Requests;

using CardsAgainstMySanity.Domain.Identity.Results;
using MediatR;

public record RefreshTokenRequest(string RefreshToken) : IRequest<RefreshTokenResult>
{
}
