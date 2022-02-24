namespace CardsAgainstMySanity.Domain.Auth.Services;

using System.Security.Claims;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.SharedKernel;
using Microsoft.Extensions.Logging;

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
        this._refreshTokenRepository = refreshTokenRepository;
        this._tokenService = tokenService;
        this._guestRepository = guestRepository;
        this._logger = logger;
    }

    public async Task<Result<(ClaimsPrincipal, string), AccessValidationError>> ValidateUserTokens(string accessToken, Guid refreshTokenId, string ipAddress)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            this._logger.LogWarning("Access token is null or empty");
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid);
        }

        if (refreshTokenId == Guid.Empty)
        {
            this._logger.LogWarning("Refresh token is empty");
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid);
        }

        var accessTokenValidationResult = this._tokenService.IsAccessTokenValid(accessToken);

        if (accessTokenValidationResult.Succeeded)
        {
            this._logger.LogInformation("Access token is valid");
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((accessTokenValidationResult.Data, accessToken));
        }
        if (accessTokenValidationResult.Error == TokenService.ValidationError.AccessTokenInvalid)
        {
            this._logger.LogWarning("Access token is invalid");
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.AccessTokenInvalid);
        }

        this._logger.LogWarning("Access token is expired and will be refreshed");
        var refreshToken = await this._refreshTokenRepository.FindByIdAsync(refreshTokenId);
        var user = await this._guestRepository.FindByIdAsync(refreshToken.UserId);
        var refreshResult = this._tokenService.Refresh(refreshToken, user, ipAddress);
        if (refreshResult.Succeeded)
        {
            this._logger.LogInformation("Token has been refreshed");
            var principal = TokenService.GeneratePrincipal(user);
            return Result<(ClaimsPrincipal, string), AccessValidationError>.Ok((principal, refreshResult.Data));
        }

        this._logger.LogWarning("Token has not been refreshed");
        return Result<(ClaimsPrincipal, string), AccessValidationError>.Fail(AccessValidationError.RefreshTokenInvalid);
    }

    public static bool IsUserAFK(string accessToken) => true;
}
