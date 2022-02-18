using System;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Infrastructure.Validators;
using CardsAgainstMySanity.SharedKernel.Validation;
using CardsAgainstMySanity.Test.Assets;
using Microsoft.Extensions.DependencyInjection;

namespace CardsAgainstMySanity.Test
{
    public class TestBase
    {
        public ServiceProvider ServiceProvider { get; set; }

        public TestBase()
        {
            var services = new ServiceCollection();

            services.AddTransient<IDateTimeProvider, FakeDateTimeProvider>(
            (_) => new FakeDateTimeProvider(DateTime.Now, DateTime.UtcNow));

            services.AddTransient<IGuestRepository, FakeGuestRepository>();
            services.AddTransient<IRefreshTokenRepository, FakeRefreshTokenRepository>();

            services.AddSingleton<TokenSettings>(
                (_) => new()
                {
                    AccessTokenExpirationInMinutes = 1,
                    GuestRefreshTokenExpirationInMinutes = 2,
                    UserAccountRefreshTokenExpirationInMinutes = 2,
                    AccessTokenIssuer = "test-issuer",
                    AccessTokenAudience = "test-audience",
                    SecretKey = "ae2b1fca515949e5d54fb22b8ed95575"
                });
                
            services.AddTransient<TokenService>((s) => new(
                s.GetRequiredService<TokenSettings>(),
                s.GetRequiredService<IRefreshTokenRepository>(),
                s.GetRequiredService<IDateTimeProvider>()));

            services.AddTransient<IModelValidator<GuestInitSessionDto>, GuestInitSessionDtoValidationAdapter>();

            this.ServiceProvider = services.BuildServiceProvider();
        }
    }
}