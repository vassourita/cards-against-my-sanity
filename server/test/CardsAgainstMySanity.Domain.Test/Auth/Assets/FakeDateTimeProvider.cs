using System;
using CardsAgainstMySanity.Domain.Providers;

namespace CardsAgainstMySanity.Domain.Test.Auth.Assets
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime now, DateTime utcNow)
        {
            Now = now;
            UtcNow = utcNow;
        }

        public DateTime Now { get; private set; }
        public DateTime UtcNow { get; private set; }
    }
}