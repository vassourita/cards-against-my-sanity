
using CardsAgainstMySanity.Domain.Auth.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using Microsoft.Extensions.Logging;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using FluentAssertions;
using CardsAgainstMySanity.Test;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class AccessServiceTest : TestBase
    {   
        private readonly ILogger<AccessService> _logger;
        public AccessServiceTest()
        {
            var _loggerFactory = new LoggerFactory();
            _logger = LoggerFactoryExtensions.CreateLogger<AccessService>(_loggerFactory);
        }
        
        #region factories
        private MakeFakeTokenService () =>
            new FakeTokenService()
        private AccessService MakeSut() => new AccessService(
            ServiceProvider.GetRequiredService<IRefreshTokenRepository>(),
            MakeFakeTokenService(),
            ServiceProvider.GetRequiredService<IGuestRepository>(),
            _logger
            );

        private IDateTimeProvider MakeDateTimeProvider() => ServiceProvider.GetRequiredService<IDateTimeProvider>();
        
        private RefreshToken MakeRefreshToken() => new RefreshToken(
            System.Guid.NewGuid(),
            MakeDateTimeProvider().UtcNow.AddMinutes(2),
            MakeDateTimeProvider()
        );

        #endregion
        #region tests
        [Fact]
        public async void ValidateUserTokens_ValidTokens_ShouldSucceed(){
            var refreshToken = MakeRefreshToken();
            // Arrange

            var sut = MakeSut();
            
            // Act

            var result = await sut.ValidateUserTokens("valid-token", refreshToken.Token);
            
            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }
        #endregion
    }
}