namespace CardsAgainstMySanity.Domain.Identity.Results;

using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.SharedKernel;

public class RefreshTokenResult : Result<(Token AccessToken, Token RefreshToken)>
{
    private RefreshTokenResult(Token accessToken, Token refreshToken) : base((accessToken, refreshToken))
    { }
    private RefreshTokenResult() : base()
    { }

    public static RefreshTokenResult Ok(Token accessToken, Token refreshToken) => new(accessToken, refreshToken);
    public static RefreshTokenResult Fail() => new();
}