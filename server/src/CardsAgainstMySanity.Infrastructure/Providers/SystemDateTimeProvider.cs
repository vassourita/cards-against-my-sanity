using CardsAgainstMySanity.Domain.Providers;

namespace CardsAgainstMySanity.Infrastructure.Providers
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}