namespace CardsAgainstMySanity.Infrastructure.IoC;

using CardsAgainstMySanity.Infrastructure.Data.EntityFramework;
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

        // Automapper

        // Configuration

        // EF
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories

        // Services

        return services;
    }
}
