namespace CardsAgainstMySanity.SharedKernel.Validation;

public interface IModelValidator<T>
{
    Result<T, ValidationErrorList> Validate(T model);
}
