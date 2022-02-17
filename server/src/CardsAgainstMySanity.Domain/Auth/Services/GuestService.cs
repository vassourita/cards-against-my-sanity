using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.SharedKernel.Validation;

namespace CardsAgainstMySanity.Domain.Auth.Services
{

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
            _guestRepository = guestRepository;
            _tokenService = tokenService;
            _dateTimeProvider = dateTimeProvider;
            _guestInitSessionDtoValidator = guestInitSessionDtoValidator;
        }

        public async Task<Result<Guest, ValidationErrorList>> InitSession(GuestInitSessionDto guestInitSessionDto)
        {
            var validationResult = _guestInitSessionDtoValidator.Validate(guestInitSessionDto);
            if (validationResult.Failed)
            {
                return Result<Guest, ValidationErrorList>.Fail(validationResult.Error);
            }

            var guest = new Guest(guestInitSessionDto.Username, guestInitSessionDto.IpAddress, "", _dateTimeProvider);

            var accessToken = _tokenService.GenerateAccessToken(guest);
            guest.SetAccessToken(accessToken, guestInitSessionDto.IpAddress);
            var refreshToken = _tokenService.GenerateRefreshToken(true);
            guest.AddRefreshToken(refreshToken);

            await _guestRepository.AddAsync(guest);
            await _guestRepository.CommitAsync();

            return Result<Guest, ValidationErrorList>.Ok(guest);
        }

        public async Task<Guest> GetGuestById(Guid id)
        {
            return await _guestRepository.FindByIdAsync(id);
        }
    }
}