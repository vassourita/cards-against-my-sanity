using System.Security.Claims;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.SharedKernel;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<AccessService> _logger;

        public AccessService(IRefreshTokenRepository refreshTokenRepository, TokenService tokenService, IGuestRepository guestRepository, ILogger<AccessService> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _guestRepository = guestRepository;
            _logger = logger;
        }

        public async Task<Result<(ClaimsPrincipal, string), AccessValidationError>> ValidateUserTokens(string accessToken, Guid refreshTokenId)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("AccessService.ValidateUserTokens: Access token is null or empty");
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid);
            }

            if (refreshTokenId == Guid.Empty)
            {
                _logger.LogWarning("AccessService.ValidateUserTokens: Refresh token is empty");
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid);
            }

            var accessTokenValidationResult = _tokenService.IsAccessTokenValid(accessToken);

            if (accessTokenValidationResult.Succeeded)
            {
                _logger.LogInformation("AccessService.ValidateUserTokens: Access token is valid");
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((accessTokenValidationResult.Data, accessToken));
            }
            if (accessTokenValidationResult.Error == TokenService.ValidationError.AccessTokenInvalid)
            {
                _logger.LogWarning("AccessService.ValidateUserTokens: Access token is invalid");
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid);
            }

            _logger.LogWarning("AccessService.ValidateUserTokens: Access token is expired and will be refreshed");
            var refreshToken = await _refreshTokenRepository.FindByIdAsync(refreshTokenId);
            var user = await _guestRepository.FindByIdAsync(refreshToken.UserId);
            var refreshResult = _tokenService.Refresh(refreshToken, user);
            if (refreshResult.Succeeded)
            {
                _logger.LogInformation("AccessService.ValidateUserTokens: Token has been refreshed");
                var principal = _tokenService.GeneratePrincipal(user);
                return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((principal, refreshResult.Data));
            }

            _logger.LogWarning("AccessService.ValidateUserTokens: Token has not been refreshed");
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid);
        }

        public bool IsUserAFK(string accessToken)
        {
            return true;
        }
    }
}