namespace CardsAgainstMySanity.Infrastructure.Validators;

using CardsAgainstMySanity.SharedKernel.Validation;
using FluentValidation.Results;

public static class ValidationFailureExtensions
{
    public static ValidationErrorList ToValidationErrorList(this IEnumerable<ValidationFailure> failure)
    {
        var list = new ValidationErrorList();
        foreach (var f in failure)
        {
            list.AddError(f.PropertyName, f.ErrorMessage);
        }
        return list;
    }
}
