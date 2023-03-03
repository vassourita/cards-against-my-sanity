namespace CardsAgainstMySanity.Domain.Identity.Handlers.Commands;

using System.Threading;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Identity.Requests;
using CardsAgainstMySanity.Domain.Identity.Results;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using MediatR;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResult>
{
    private readonly AccessTokenFactory _accessTokenFactory;
    private readonly RefreshTokenFactory _refreshTokenFactory;
    private readonly TokenValidator _tokenValidator;

    public RefreshTokenHandler(
        AccessTokenFactory accessTokenFactory,
        RefreshTokenFactory refreshTokenFactory,
        TokenValidator tokenValidator)
    {
        _accessTokenFactory = accessTokenFactory;
        _refreshTokenFactory = refreshTokenFactory;
        _tokenValidator = tokenValidator;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _tokenValidator.ValidateAsync(request.RefreshToken, true);

        if (user == null)
            return RefreshTokenResult.Fail();

        var accessToken = await _accessTokenFactory.GenerateAsync(user);
        var refreshToken = await _refreshTokenFactory.GenerateAsync(user);

        return RefreshTokenResult.Ok(accessToken, refreshToken);
    }
}