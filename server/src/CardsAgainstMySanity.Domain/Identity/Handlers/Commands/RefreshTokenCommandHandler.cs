namespace CardsAgainstMySanity.Domain.Identity.Handlers.Commands;

using System.Threading;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Identity.Requests;
using CardsAgainstMySanity.Domain.Identity.Results;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using MediatR;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResult>
{
    private readonly IGuestRepository _guestRepository;
    private readonly AccessTokenFactory _accessTokenFactory;
    private readonly RefreshTokenFactory _refreshTokenFactory;
    private readonly TokenValidator _tokenValidator;

    public RefreshTokenHandler(
        AccessTokenFactory accessTokenFactory,
        RefreshTokenFactory refreshTokenFactory,
        TokenValidator tokenValidator,
        IGuestRepository guestRepository)
    {
        _accessTokenFactory = accessTokenFactory;
        _refreshTokenFactory = refreshTokenFactory;
        _tokenValidator = tokenValidator;
        _guestRepository = guestRepository;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return RefreshTokenResult.Fail();

        Guest guest;
        if (_tokenValidator.TryValidate(request.RefreshToken, true, out var principalId))
        {
            guest = await _guestRepository.FindByIdAsync(principalId);

            if (!guest.IsActive)
                return RefreshTokenResult.Fail();

            if (await TryDeactivateGuestByInactivity(guest))
                return RefreshTokenResult.Fail();

            var accessToken = await _accessTokenFactory.GenerateAsync(guest);
            var refreshToken = await _refreshTokenFactory.GenerateAsync(guest);

            guest.UpdateLastActivityDate();
            await _guestRepository.UpdateAsync(guest);
            await _guestRepository.CommitAsync();

            return RefreshTokenResult.Ok(accessToken, refreshToken);
        }

        if (principalId == null)
            return RefreshTokenResult.Fail();

        guest = await _guestRepository.FindByIdAsync(principalId);
        if (!guest.IsActive)
            return RefreshTokenResult.Fail();

        await TryDeactivateGuestByInactivity(guest);
        return RefreshTokenResult.Fail();
    }

    private async Task<bool> TryDeactivateGuestByInactivity(Guest guest)
    {
        if (guest.LastActivityDate.AddHours(1) > DateTime.UtcNow)
        {
            guest.Deactivate();
            await _guestRepository.UpdateAsync(guest);
            await _guestRepository.CommitAsync();
            return true;
        }
        return false;
    }
}
