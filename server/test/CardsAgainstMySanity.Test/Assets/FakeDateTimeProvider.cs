namespace CardsAgainstMySanity.Test.Assets;

using System;
using CardsAgainstMySanity.Domain.Providers;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public FakeDateTimeProvider(DateTime now, DateTime utcNow)
    {
        this.Now = now;
        this.UtcNow = utcNow;
    }

    public DateTime Now { get; private set; }
    public DateTime UtcNow { get; private set; }
}
