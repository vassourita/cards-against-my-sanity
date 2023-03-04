namespace CardsAgainstMySanity.Domain.Identity.Handlers.Commands;

using System.Threading;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Identity.Requests;
using CardsAgainstMySanity.Domain.Identity.Results;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.SharedKernel.Validation;
using MediatR;

public class CreateGuestSessionHandler : IRequestHandler<CreateGuestSessionRequest, CreateGuestSessionResult>
{
    private readonly AccessTokenFactory _accessTokenFactory;
    private readonly RefreshTokenFactory _refreshTokenFactory;
    private readonly IGuestRepository _guestRepository;

    public CreateGuestSessionHandler(
        AccessTokenFactory accessTokenFactory,
        RefreshTokenFactory refreshTokenFactory,
        IGuestRepository guestRepository)
    {
        _accessTokenFactory = accessTokenFactory;
        _refreshTokenFactory = refreshTokenFactory;
        _guestRepository = guestRepository;
    }

    public async Task<CreateGuestSessionResult> Handle(CreateGuestSessionRequest request, CancellationToken cancellationToken)
    {
        var guestExists = await _guestRepository.FindByNameAsync(request.Name);
        if (guestExists is not null)
            return CreateGuestSessionResult.Fail(new ValidationErrorList("Name", "Name is already taken"));

        var guest = new Guest(request.Name);
        var accessToken = await _accessTokenFactory.GenerateAsync(guest);
        var refreshToken = await _refreshTokenFactory.GenerateAsync(guest);

        return CreateGuestSessionResult.Ok(guest, accessToken, refreshToken);
    }
}
