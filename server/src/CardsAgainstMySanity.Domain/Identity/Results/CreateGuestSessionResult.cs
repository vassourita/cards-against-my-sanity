namespace CardsAgainstMySanity.Domain.Identity.Results;

using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.SharedKernel.Validation;

public class CreateGuestSessionResult : Result<(Guest Guest, Token AccessToken, Token RefreshToken), ValidationErrorList>
{
    private CreateGuestSessionResult(Guest guest, Token accessToken, Token refreshToken) : base((guest, accessToken, refreshToken))
    { }
    private CreateGuestSessionResult(ValidationErrorList errors) : base(errors)
    { }

    public static CreateGuestSessionResult Ok(Guest guest, Token accessToken, Token refreshToken)
        => new(guest, accessToken, refreshToken);

    public static CreateGuestSessionResult Fail(ValidationErrorList errors)
        => new(errors);
}