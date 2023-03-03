namespace CardsAgainstMySanity.SharedKernel.Validation;

using System.Globalization;

public class ValidationErrorList : Dictionary<string, List<string>>
{
    public ValidationErrorList AddError(string propertyName, string errorMessage)
    {
        var prop = propertyName.ToLower(CultureInfo.InvariantCulture);
        try
        {
            this[prop].Add(errorMessage);
            return this;
        }
        catch (KeyNotFoundException)
        {
            this[prop] = new List<string>
                {
                    errorMessage
                };
            return this;
        }
    }

    public ValidationErrorList() { }
    public ValidationErrorList(string propertyName, string errorMessage) => AddError(propertyName, errorMessage);
}
