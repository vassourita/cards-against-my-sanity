using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;

namespace CardsAgainstMySanity.Domain.Auth.Services
{

    public class GuestService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly TokenService _tokenService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GuestService(IGuestRepository guestRepository, TokenService tokenService, IDateTimeProvider dateTimeProvider)
        {
            _guestRepository = guestRepository;
            _tokenService = tokenService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<Guest>> InitSession(GuestInitSessionDto guestInitSessionDto, string ipAddress)
        {
            var guest = new Guest(guestInitSessionDto.Username, ipAddress, "", _dateTimeProvider);

            var accessToken = _tokenService.GenerateAccessToken(guest);
            guest.SetAccessToken(accessToken, ipAddress);
            var refreshToken = _tokenService.GenerateRefreshToken(true);
            guest.AddRefreshToken(refreshToken);

            await _guestRepository.AddAsync(guest);
            await _guestRepository.CommitAsync();

            return Result<Guest>.Ok(guest);
        }

        public async Task<Guest> GetGuestById(Guid id)
        {
            return await _guestRepository.FindByIdAsync(id);
        }
    }
}