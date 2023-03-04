namespace CardsAgainstMySanity.Infrastructure.IoC;

using CardsAgainstMySanity.Domain.Identity;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCardsAgainstMySanity(this IServiceCollection services, IConfiguration configuration)
    {
        // Logging
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
            config.SetMinimumLevel(LogLevel.Trace);
        });

        // Configuration
        services.AddSingleton(_ =>
        {
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind("JwtConfiguration", jwtConfiguration);
            return jwtConfiguration;
        });

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));

        // EF
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("EFConnection")));

        // MediatR
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Guest>());

        // Automapper

        // Providers
        services.AddScoped<AccessTokenFactory>();
        services.AddScoped<RefreshTokenFactory>();
        services.AddScoped<TokenValidator>();

        // Repositories

        // Services

        return services;
    }
}
