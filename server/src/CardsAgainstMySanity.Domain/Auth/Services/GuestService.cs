namespace CardsAgainstMySanity.Domain.Auth.Services;

using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.SharedKernel.Validation;

public class GuestService
{
    private readonly IGuestRepository _guestRepository;
    private readonly TokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IModelValidator<GuestInitSessionDto> _guestInitSessionDtoValidator;

    public GuestService(IGuestRepository guestRepository,
                        TokenService tokenService,
                        IDateTimeProvider dateTimeProvider,
                        IModelValidator<GuestInitSessionDto> guestInitSessionDtoValidator)
    {
        this._guestRepository = guestRepository;
        this._tokenService = tokenService;
        this._dateTimeProvider = dateTimeProvider;
        this._guestInitSessionDtoValidator = guestInitSessionDtoValidator;
    }

    public async Task<Result<Guest, ValidationErrorList>> InitSession(GuestInitSessionDto guestInitSessionDto)
    {
        var validationResult = this._guestInitSessionDtoValidator.Validate(guestInitSessionDto);
        if (validationResult.Failed)
        {
            return Result<Guest, ValidationErrorList>.Fail(validationResult.Error);
        }

        var guest = new Guest(guestInitSessionDto.Username, guestInitSessionDto.IpAddress, "", this._dateTimeProvider);

        var accessToken = this._tokenService.GenerateAccessToken(guest);
        guest.SetAccessToken(accessToken, guestInitSessionDto.IpAddress);
        var refreshToken = this._tokenService.GenerateRefreshToken(true);
        guest.AddRefreshToken(refreshToken);

        await this._guestRepository.AddAsync(guest);
        await this._guestRepository.CommitAsync();

        return Result<Guest, ValidationErrorList>.Ok(guest);
    }

    public async Task<Guest> GetGuestById(Guid id) => await this._guestRepository.FindByIdAsync(id);
}
