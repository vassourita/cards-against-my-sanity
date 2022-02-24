using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Test.Assets;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Test;
using Microsoft.Extensions.DependencyInjection;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using Moq;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class TokenServiceTest : TestBase
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TokenSettings _tokenSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public TokenServiceTest()
        {
            _dateTimeProvider = ServiceProvider.GetRequiredService<IDateTimeProvider>();
            _tokenSettings = ServiceProvider.GetRequiredService<TokenSettings>();
            _refreshTokenRepository = ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
        }
        #region factories
        private TokenService MakeSut()
            => new(
                _tokenSettings,
                _refreshTokenRepository,
                _dateTimeProvider);

        private TestUser MakeUser()
            => new(
                id: Guid.NewGuid(),
                username: "test-username",
                ipAddress: "test-ip-address",
                avatarUrl: "test-avatar-url"
            );
        #endregion

        #region testCases
        [Fact(DisplayName = "#GetClaims should return valid claims")]
        public void GetClaims_ShouldReturnCorrectClaims()
        {
            // Arrange
            var user = MakeUser();
            var sut = MakeSut();

            // Act
            var claims = sut.GetClaims(user);

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
        public void GeneratePrincipal_ShouldReturnValidPrincipal()
        {
            // Arrange
            var sut = MakeSut();
            var user = MakeUser();

            // Act
            var principal = sut.GeneratePrincipal(user);

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
        public void GenerateAccessToken_ShouldReturnValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var user = MakeUser();

            // Act
            var token = sut.GenerateAccessToken(user);
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
                    ValidateIssuer = false, //
                    ValidateAudience = false, //
                    ValidateLifetime = false, //
                }, out var validatedToken);
            var jwt = validatedToken as JwtSecurityToken;

            // Assert
            jwt.Issuer.Should().Be(_tokenSettings.AccessTokenIssuer);

            jwt.Audiences.Should().Contain(_tokenSettings.AccessTokenAudience);

            var validFromNoMs = RemoveMs(_dateTimeProvider.UtcNow);

            jwt.ValidFrom
                .Should().Be(validFromNoMs);

            var validToNoMs = RemoveMs(_dateTimeProvider.UtcNow.AddMinutes(_tokenSettings.AccessTokenExpirationInMinutes));
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

        private DateTime RemoveMs(DateTime dt)
            => DateTime.Parse(dt.ToString("yyyy-MM-dd HH:mm:ss"));

        [Fact(DisplayName = "#GenerateRefreshToken should return a valid token")]
        public void GenerateRefreshToken_ShouldReturnValidToken()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var token = sut.GenerateRefreshToken(true);

            // Assert
            token.Should().NotBeNull();
            token.ExpiresAt
                .Should().Be(_dateTimeProvider.UtcNow.AddMinutes(_tokenSettings.UserAccountRefreshTokenExpirationInMinutes));
            token.CreatedAt
                .Should().Be(_dateTimeProvider.UtcNow);
            token.UserId
                .Should().Be(Guid.Empty);
        }

        [Fact(DisplayName = "#Refresh should return a valid token for a user when a valid refresh token is provided")]
        public void Refresh_ValidTokens_ShouldRefresh()
        {
            // Arrange
            var sut = MakeSut();
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
        public void Refresh_InvalidToken_ShouldNotRefresh()
        {
            // Arrange
            var sut = MakeSut();
            var user = MakeUser();
            var token = new RefreshToken(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow.AddMinutes(-1),
                _dateTimeProvider);

            // Act
            var newTokenResult = sut.Refresh(token, user, "127.0.0.1");

            // Assert
            newTokenResult.Failed.Should().BeTrue();
            newTokenResult.Data
                .Should().Be(default(string));
        }

        [Fact(DisplayName = "#Refresh should not return a valid token for a user when a invalid ip address is provided")]
        public void Refresh_InvalidIpAddress_ShouldNotRefresh()
        {
            // Arrange
            var sut = MakeSut();
            var user = MakeUser();
            var token = new RefreshToken(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow.AddMinutes(-1),
                _dateTimeProvider);

            // Act
            var newTokenResult = sut.Refresh(token, user, "some-ip");

            // Assert
            newTokenResult.Failed.Should().BeTrue();
            newTokenResult.Data
                .Should().Be(default(string));
        }

        [Fact(DisplayName = "#IsAccessTokenValid should return true when a valid token is provided")]
        public void IsAccessTokenValid_ShouldReturnTrueForValidToken()
        {
            // Arrange
            var mockConstructor = new object[] {
                _tokenSettings,
                _refreshTokenRepository,
                _dateTimeProvider
            };
            var sutMock = new Mock<TokenService>(mockConstructor);
            sutMock.CallBase = true; // partial mock
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
        public void IsAccessTokenValid_ShouldReturnFalseForInvalidToken()
        {
            // Arrange
            var mockConstructor = new object[] {
                _tokenSettings,
                _refreshTokenRepository,
                _dateTimeProvider
            };
            var sutMock = new Mock<TokenService>(mockConstructor);
            sutMock.CallBase = true; // partial mock
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
        public void IsAccessTokenValid_ShouldCallValidateJWT()
        {
            // Arrange
            var mockConstructor = new object[] {
                _tokenSettings,
                _refreshTokenRepository,
                _dateTimeProvider
            };
            var sutMock = new Mock<TokenService>(mockConstructor);
            sutMock.CallBase = true; // partial mock
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
        public void ValidateJWT_ShouldReturnClaimsPrincipalForValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var token = sut.GenerateAccessToken(MakeUser());
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
                ValidateIssuer = false, //
                ValidateAudience = false, //
                ValidateLifetime = false, //
            };
            // Act & Assert
            var result = sut.ValidateJWT(token, validationParameters);
            result.Should().NotBeNull().And.BeOfType<ClaimsPrincipal>();
        }
        [Fact(DisplayName = "#ValidateJWT should throw an ArgumentException when an token with an invalid segmentation is provided")]
        public void ValidateJWT_InvalidTokenSegmentation_ShouldThrowArgumentException()
        {
            // Arrange
            var sut = MakeSut();
            var token = "invalid token!";
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
                ValidateIssuer = false, //
                ValidateAudience = false, //
                ValidateLifetime = false, //
            };

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => sut.ValidateJWT(token, validationParameters));
        }

        [Fact(DisplayName = "#IsRefreshTokenValid should return true when a valid token is provided")]
        public void IsRefreshTokenValid_ShouldReturnTrueForValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var token = sut.GenerateRefreshToken(true);

            // Act
            var isValid = sut.IsRefreshTokenValid(token);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact(DisplayName = "#IsRefreshTokenValid should return false when an invalid token is provided")]
        public void IsRefreshTokenValid_ShouldReturnFalseForInvalidToken()
        {
            // Arrange
            var sut = MakeSut();
            var token1 = new RefreshToken(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow.AddMinutes(-1),
                _dateTimeProvider);
            RefreshToken token2 = null;

            // Act
            var isValid1 = sut.IsRefreshTokenValid(token1);
            var isValid2 = sut.IsRefreshTokenValid(token2);

            // Assert
            isValid1.Should().BeFalse();
            isValid2.Should().BeFalse();
        }
        #endregion
    }
}