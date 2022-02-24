namespace CardsAgainstMySanity.Domain.Test.Auth.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Test;
using CardsAgainstMySanity.Test.Assets;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

public class TokenServiceTest : TestBase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly TokenSettings _tokenSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public TokenServiceTest()
    {
        this._dateTimeProvider = this.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        this._tokenSettings = this.ServiceProvider.GetRequiredService<TokenSettings>();
        this._refreshTokenRepository = this.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
    }
    #region factories
    private TokenService MakeSut()
        => new(
            this._tokenSettings,
            this._refreshTokenRepository,
            this._dateTimeProvider);

    private static TestUser MakeUser()
        => new(
            id: Guid.NewGuid(),
            username: "test-username",
            ipAddress: "test-ip-address",
            avatarUrl: "test-avatar-url"
        );
    #endregion

    #region testCases
    [Fact(DisplayName = "#GetClaims should return valid claims")]
    public void GetClaimsShouldReturnCorrectClaims()
    {
        // Arrange
        var user = MakeUser();
        var sut = this.MakeSut();

        // Act
        var claims = TokenService.GetClaims(user);

        // Assert
        claims
            .Should().HaveCount(2)
            .And.Contain(
                c => c.Type == ClaimTypes.NameIdentifier &&
                c.Value == user.Id.ToString())
            .And.Contain(
                c => c.Type == ClaimTypes.Name &&
                c.Value == user.Username);
    }

    [Fact(DisplayName = "#GeneratePrincipal should return a valid principal")]
    public void GeneratePrincipalShouldReturnValidPrincipal()
    {
        // Arrange
        var sut = this.MakeSut();
        var user = MakeUser();

        // Act
        var principal = TokenService.GeneratePrincipal(user);

        // Assert
        principal.Identity.IsAuthenticated.Should().BeTrue();
        principal.Identity.Name.Should().Be(user.Username);
        principal.Claims
            .Should().HaveCount(2);
        principal.Claims
            .Should().HaveCount(2)
            .And.Contain(
                c => c.Type == ClaimTypes.NameIdentifier &&
                c.Value == user.Id.ToString())
            .And.Contain(
                c => c.Type == ClaimTypes.Name &&
                c.Value == user.Username);
        principal.Identity.AuthenticationType.Should().Be("Bearer");
    }

    [Fact(DisplayName = "#GenerateToken should return a valid token")]
    public void GenerateAccessTokenShouldReturnValidToken()
    {
        // Arrange
        var sut = this.MakeSut();
        var user = MakeUser();

        // Act
        var token = sut.GenerateAccessToken(user);
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
            new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._tokenSettings.SecretKey)),
                ValidateIssuer = false, //
                ValidateAudience = false, //
                ValidateLifetime = false, //
            }, out var validatedToken);
        var jwt = validatedToken as JwtSecurityToken;

        // Assert
        jwt.Issuer.Should().Be(this._tokenSettings.AccessTokenIssuer);

        jwt.Audiences.Should().Contain(this._tokenSettings.AccessTokenAudience);

        var validFromNoMs = RemoveMs(this._dateTimeProvider.UtcNow);

        jwt.ValidFrom
            .Should().Be(validFromNoMs);

        var validToNoMs = RemoveMs(this._dateTimeProvider.UtcNow.AddMinutes(this._tokenSettings.AccessTokenExpirationInMinutes));
        jwt.ValidTo
            .Should().Be(validToNoMs);

        jwt.Claims
            .Should().HaveCount(6)
            .And.Contain(
                c => c.Type == ClaimTypes.NameIdentifier &&
                c.Value == user.Id.ToString())
            .And.Contain(
                c => c.Type == ClaimTypes.Name &&
                c.Value == user.Username)
            .And.Contain(
                c => c.Type == "nbf")
            .And.Contain(
                c => c.Type == "exp")
            .And.Contain(
                c => c.Type == "iss")
            .And.Contain(
                c => c.Type == "aud");
    }

    private static DateTime RemoveMs(DateTime dt)
        => DateTime.Parse(dt.ToString("yyyy-MM-dd HH:mm:ss"));

    [Fact(DisplayName = "#GenerateRefreshToken should return a valid token")]
    public void GenerateRefreshTokenShouldReturnValidToken()
    {
        // Arrange
        var sut = this.MakeSut();

        // Act
        var token = sut.GenerateRefreshToken(true);

        // Assert
        token.Should().NotBeNull();
        token.ExpiresAt
            .Should().Be(this._dateTimeProvider.UtcNow.AddMinutes(this._tokenSettings.UserAccountRefreshTokenExpirationInMinutes));
        token.CreatedAt
            .Should().Be(this._dateTimeProvider.UtcNow);
        token.UserId
            .Should().Be(Guid.Empty);
    }

    [Fact(DisplayName = "#Refresh should return a valid token for a user when a valid refresh token is provided")]
    public void RefreshValidTokensShouldRefresh()
    {
        // Arrange
        var sut = this.MakeSut();
        var user = MakeUser();
        var token = sut.GenerateRefreshToken(true);

        // Act
        var newTokenResult = sut.Refresh(token, user, "127.0.0.1");

        // Assert
        newTokenResult.Succeeded.Should().BeTrue();
        newTokenResult.Data
            .Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "#Refresh should not return a valid token for a user when a invalid refresh token is provided")]
    public void RefreshInvalidTokenShouldNotRefresh()
    {
        // Arrange
        var sut = this.MakeSut();
        var user = MakeUser();
        var token = new RefreshToken(
            Guid.NewGuid(),
            this._dateTimeProvider.UtcNow.AddMinutes(-1),
            this._dateTimeProvider);

        // Act
        var newTokenResult = sut.Refresh(token, user, "127.0.0.1");

        // Assert
        newTokenResult.Failed.Should().BeTrue();
        newTokenResult.Data
            .Should().Be(default);
    }

    [Fact(DisplayName = "#Refresh should not return a valid token for a user when a invalid ip address is provided")]
    public void RefreshInvalidIpAddressShouldNotRefresh()
    {
        // Arrange
        var sut = this.MakeSut();
        var user = MakeUser();
        var token = new RefreshToken(
            Guid.NewGuid(),
            this._dateTimeProvider.UtcNow.AddMinutes(-1),
            this._dateTimeProvider);

        // Act
        var newTokenResult = sut.Refresh(token, user, "some-ip");

        // Assert
        newTokenResult.Failed.Should().BeTrue();
        newTokenResult.Data
            .Should().Be(default);
    }

    [Fact(DisplayName = "#IsAccessTokenValid should return true when a valid token is provided")]
    public void IsAccessTokenValidShouldReturnTrueForValidToken()
    {
        // Arrange
        var mockConstructor = new object[] {
                this._tokenSettings,
                this._refreshTokenRepository,
                this._dateTimeProvider
            };
        var sutMock = new Mock<TokenService>(mockConstructor)
        {
            CallBase = true // partial mock
        };
        sutMock.Setup(x => x.ValidateJWT("valid-token", new TokenValidationParameters()))
        .Returns(new ClaimsPrincipal());
        var sut = sutMock.Object;

        var user = MakeUser();
        var token = sut.GenerateAccessToken(user);

        // Act
        var isValidResult = sut.IsAccessTokenValid(token);

        // Assert
        isValidResult.Succeeded.Should().BeTrue();
    }

    [Fact(DisplayName = "#IsAccessTokenValid should return false when an invalid token is provided")]
    public void IsAccessTokenValidShouldReturnFalseForInvalidToken()
    {
        // Arrange
        var mockConstructor = new object[] {
                this._tokenSettings,
                this._refreshTokenRepository,
                this._dateTimeProvider
            };
        var sutMock = new Mock<TokenService>(mockConstructor)
        {
            CallBase = true // partial mock
        };
        sutMock.Setup(x => x.ValidateJWT("valid-token", new TokenValidationParameters()))
        .Throws(new SecurityTokenExpiredException());
        var sut = sutMock.Object;


        var token = "somejwttoken";

        // Act
        var isValidResult = sut.IsAccessTokenValid(token);

        // Assert
        isValidResult.Succeeded.Should().BeFalse();
    }

    [Fact(DisplayName = "#IsAccessTokenValid should make a call to #ValidateJWT")]
    public void IsAccessTokenValidShouldCallValidateJWT()
    {
        // Arrange
        var mockConstructor = new object[] {
                this._tokenSettings,
                this._refreshTokenRepository,
                this._dateTimeProvider
            };
        var sutMock = new Mock<TokenService>(mockConstructor)
        {
            CallBase = true // partial mock
        };
        sutMock.Setup(x => x.ValidateJWT("valid-token", new TokenValidationParameters()))
        .Returns(new ClaimsPrincipal());
        var sut = sutMock.Object;


        var token = "somejwttoken";
        // Act
        sut.IsAccessTokenValid(token);

        // Assert
        sutMock.Verify(x => x.ValidateJWT(token, It.IsAny<TokenValidationParameters>()), Times.Once);
    }

    [Fact(DisplayName = "#ValidateJWT should return a ClaimsPrincipal when a valid token is provided")]
    public void ValidateJWTShouldReturnClaimsPrincipalForValidToken()
    {
        // Arrange
        var sut = this.MakeSut();
        var token = sut.GenerateAccessToken(MakeUser());
        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._tokenSettings.SecretKey)),
            ValidateIssuer = false, //
            ValidateAudience = false, //
            ValidateLifetime = false, //
        };
        // Act & Assert
        var result = sut.ValidateJWT(token, validationParameters);
        result.Should().NotBeNull().And.BeOfType<ClaimsPrincipal>();
    }
    [Fact(DisplayName = "#ValidateJWT should throw an ArgumentException when an token with an invalid segmentation is provided")]
    public void ValidateJWTInvalidTokenSegmentationShouldThrowArgumentException()
    {
        // Arrange
        var sut = this.MakeSut();
        var token = "invalid token!";
        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._tokenSettings.SecretKey)),
            ValidateIssuer = false, //
            ValidateAudience = false, //
            ValidateLifetime = false, //
        };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => sut.ValidateJWT(token, validationParameters));
    }

    [Fact(DisplayName = "#IsRefreshTokenValid should return true when a valid token is provided")]
    public void IsRefreshTokenValidShouldReturnTrueForValidToken()
    {
        // Arrange
        var sut = this.MakeSut();
        var token = sut.GenerateRefreshToken(true);

        // Act
        var isValid = TokenService.IsRefreshTokenValid(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "#IsRefreshTokenValid should return false when an invalid token is provided")]
    public void IsRefreshTokenValidShouldReturnFalseForInvalidToken()
    {
        // Arrange
        var sut = this.MakeSut();
        var token1 = new RefreshToken(
            Guid.NewGuid(),
            this._dateTimeProvider.UtcNow.AddMinutes(-1),
            this._dateTimeProvider);
        RefreshToken token2 = null;

        // Act
        var isValid1 = TokenService.IsRefreshTokenValid(token1);
        var isValid2 = TokenService.IsRefreshTokenValid(token2);

        // Assert
        isValid1.Should().BeFalse();
        isValid2.Should().BeFalse();
    }
    #endregion
}
