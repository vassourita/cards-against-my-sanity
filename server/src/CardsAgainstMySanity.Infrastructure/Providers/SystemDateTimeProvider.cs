namespace CardsAgainstMySanity.Infrastructure.Providers;

using CardsAgainstMySanity.Domain.Providers;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
