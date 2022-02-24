namespace CardsAgainstMySanity.Infrastructure.IoC;

using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Mappings;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories;
using CardsAgainstMySanity.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCardsAgainstMySanity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
            config.SetMinimumLevel(LogLevel.Trace);
        });

        // Providers
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // Automapper
        services.AddAutoMapper(config => config.AddProfile<UserMappingProfile>());

        // Configuration
        services.AddSingleton((_) =>
        {
            var tokenSettings = new TokenSettings();
            configuration.Bind("TokenSettings", tokenSettings);
            return tokenSettings;
        });

        // EF
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IGuestRepository, EFGuestRepository>();
        services.AddScoped<IRefreshTokenRepository, EFRefreshTokenRepository>();

        // Services
        services.AddScoped<GuestService>();
        services.AddScoped<TokenService>();
        services.AddScoped<AccessService>();

        return services;
    }
}
