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

        private readonly TokenService _tokenService;

        public AccessService(IRefreshTokenRepository refreshTokenRepository, TokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        public Task<Result<(ClaimsPrincipal, string), AccessValidationError>> ValidateUserTokens(string accessToken, string refreshTokenId)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshTokenId))
            {
                return Task.FromResult(Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid));
            }

            if (_tokenService.IsAccessTokenValid(accessToken, out var principal))
            {
                return Task.FromResult(Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((principal, accessToken)));
            }
            return Task.FromResult(Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid));
        }

        public bool IsUserAFK(string accessToken)
        {
            return true;
        }
    }
}