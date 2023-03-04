namespace CardsAgainstMySanity.Domain.Identity.Requests;

using CardsAgainstMySanity.Domain.Identity.Results;
using MediatR;

public class CreateGuestSessionRequest : IRequest<CreateGuestSessionResult>
{
    public string Name { get; set; }
}
