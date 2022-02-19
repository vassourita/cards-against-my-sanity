using Moq;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Test;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CardsAgainstMySanity.SharedKernel;
using System.Security.Claims;

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
        private Mock<TokenService> MakeMockTokenService()
        {
            var mock = new Mock<TokenService>();
            mock.Setup(x => x.IsAccessTokenValid("valid-token"))
            .Returns(Result<ClaimsPrincipal, TokenService.ValidationError>.Ok(new ClaimsPrincipal()));
            return mock;
        }

        private AccessService MakeSut() => new AccessService(
            ServiceProvider.GetRequiredService<IRefreshTokenRepository>(),
            MakeMockTokenService().Object,
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
        public async void ValidateUserTokens_ValidTokens_ShouldSucceed()
        {
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