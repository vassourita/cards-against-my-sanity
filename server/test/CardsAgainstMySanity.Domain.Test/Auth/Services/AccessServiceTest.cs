namespace CardsAgainstMySanity.Domain.Test.Auth.Services;

using System.Security.Claims;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.Test;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AccessServiceTest : TestBase
{
    private readonly ILogger<AccessService> _logger;
    public AccessServiceTest()
    {
        var _loggerFactory = new LoggerFactory();
        this._logger = LoggerFactoryExtensions.CreateLogger<AccessService>(_loggerFactory);
    }

    #region factories
    private static Mock<TokenService> MakeMockTokenService()
    {
        var mock = new Mock<TokenService>();
        mock.Setup(x => x.IsAccessTokenValid("valid-token"))
            .Returns(Result<ClaimsPrincipal, TokenService.ValidationError>.Ok(new ClaimsPrincipal()));
        return mock;
    }

    private AccessService MakeSut() => new(
        this.ServiceProvider.GetRequiredService<IRefreshTokenRepository>(),
        MakeMockTokenService().Object,
        this.ServiceProvider.GetRequiredService<IGuestRepository>(),
        this._logger
        );

    private IDateTimeProvider MakeDateTimeProvider() => this.ServiceProvider.GetRequiredService<IDateTimeProvider>();

    private RefreshToken MakeRefreshToken() => new(
        System.Guid.NewGuid(),
        this.MakeDateTimeProvider().UtcNow.AddMinutes(2),
        this.MakeDateTimeProvider()
    );

    #endregion
    #region tests
    [Fact(DisplayName = "#ValidateUserTokens should return a valid access token when valid tokens are provided")]
    public async void ValidateUserTokensValidArgumentsShouldSucceed()
    {
        var refreshToken = this.MakeRefreshToken();

        // Arrange
        var sut = this.MakeSut();

        // Act
        var result = await sut.ValidateUserTokens("valid-token", refreshToken.Token, "127.0.0.1");

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
    #endregion
}
