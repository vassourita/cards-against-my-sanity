namespace CardsAgainstMySanity.Domain.Providers;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
