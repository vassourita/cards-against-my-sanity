using System.Security.Claims;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.SharedKernel;

namespace CardsAgainstMySanity.Domain.Auth.Services
{
    public enum AccessValidationError
    {
        AccessTokenInvalid,
        RefreshTokenInvalid,
    }

    public class AccessService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly TokenService _tokenService;

        public AccessService(IRefreshTokenRepository refreshTokenRepository, TokenService tokenService, IGuestRepository guestRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _guestRepository = guestRepository;
        }

        public async Task<Result<(ClaimsPrincipal, string), AccessValidationError>> ValidateUserTokens(string accessToken, Guid refreshTokenId)
        {
            if (string.IsNullOrEmpty(accessToken) || refreshTokenId == Guid.Empty)
            {
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid);
            }

            if (_tokenService.IsAccessTokenValid(accessToken, out var principal))
            {
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((principal, accessToken));
            }

            var refreshToken = await _refreshTokenRepository.FindByIdAsync(refreshTokenId);
            var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _guestRepository.FindByIdAsync(userId);
            var refreshResult = _tokenService.Refresh(refreshToken, user);
            if (refreshResult.Succeeded)
            {
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((principal, refreshResult.Data));
            }

            return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid);
        }

        public bool IsUserAFK(string accessToken)
        {
            return true;
        }
    }
}