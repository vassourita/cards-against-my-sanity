using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Mappings;

namespace CardsAgainstMySanity.Infrastructure.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCardsAgainstMySanity(this IServiceCollection services, IConfiguration configuration)
        {
            // Automapperw
            services.AddAutoMapper(config =>
            {
                config.AddProfile<UserMappingProfile>();
            });

            // Configuration
            services.AddSingleton<TokenSettings>((_) =>
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
}