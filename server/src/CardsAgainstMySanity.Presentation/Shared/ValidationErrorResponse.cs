namespace CardsAgainstMySanity.Presentation.Shared;

using CardsAgainstMySanity.SharedKernel.Validation;

public class ValidationErrorResponse
{
    public ValidationErrorResponse(ValidationErrorList errors) => Errors = errors;
    public ValidationErrorList Errors { get; private set; }
    public string Message => "One or more validation errors occurred.";
}
