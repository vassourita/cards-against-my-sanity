namespace CardsAgainstMySanity.SharedKernel.Validation
{
    public class ValidationErrorList : Dictionary<string, List<string>>
    {
        public ValidationErrorList AddError(string propertyName, string errorMessage)
        {
            var prop = propertyName.ToLower();
            try
            {
                this[prop].Add(errorMessage);
                return this;
            }
            catch (KeyNotFoundException)
            {
                this[prop] = new List<string>();
                this[prop].Add(errorMessage);
                return this;
            }
        }

        public ValidationErrorList() { }
    }
}