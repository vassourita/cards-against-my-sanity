using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Test.Auth.Assets;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Providers;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class TokenServiceTest
    {
        private TokenSettings MakeSutSettings()
            => new()
            {
                AccessTokenExpirationInMinutes = 1,
                GuestRefreshTokenExpirationInMinutes = 2,
                UserAccountRefreshTokenExpirationInMinutes = 2,
                AccessTokenIssuer = "test-issuer",
                AccessTokenAudience = "test-audience",
                SecretKey = "ae2b1fca515949e5d54fb22b8ed95575"
            };

        private TokenService MakeSut()
            => new(
                MakeSutSettings(),
                new FakeRefreshTokenRepository(),
                _dateTimeProvider);

        private TestUser MakeUser()
            => new(
                id: Guid.NewGuid(),
                username: "test-username",
                ipAddress: "test-ip-address",
                avatarUrl: "test-avatar-url"
            );

        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenServiceTest()
        {
            _dateTimeProvider = new FakeDateTimeProvider(DateTime.Now, DateTime.UtcNow);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void GenerateAccessToken_ShouldReturnValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var user = MakeUser();
            var settings = MakeSutSettings();

            // Act
            var token = sut.GenerateAccessToken(user);
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                    ValidateIssuer = false, //
                    ValidateAudience = false, //
                    ValidateLifetime = false, //
                }, out var validatedToken);
            var jwt = validatedToken as JwtSecurityToken;

            // Assert
            jwt.Issuer.Should().Be(settings.AccessTokenIssuer);

            jwt.Audiences.Should().Contain(settings.AccessTokenAudience);

            var validFromNoMs = RemoveMs(_dateTimeProvider.UtcNow);

            jwt.ValidFrom
                .Should().Be(validFromNoMs);

            var validToNoMs = RemoveMs(_dateTimeProvider.UtcNow.AddMinutes(settings.AccessTokenExpirationInMinutes));
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

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var settings = MakeSutSettings();

            // Act
            var token = sut.GenerateRefreshToken(true);

            // Assert
            token.Should().NotBeNull();
            token.ExpiresAt
                .Should().Be(_dateTimeProvider.UtcNow.AddMinutes(settings.UserAccountRefreshTokenExpirationInMinutes));
            token.CreatedAt
                .Should().Be(_dateTimeProvider.UtcNow);
            token.UserId
                .Should().Be(Guid.Empty);
        }

        [Fact]
        public void Refresh_ShouldRefreshValidTokens()
        {
            // Arrange
            var sut = MakeSut();
            var settings = MakeSutSettings();
            var user = MakeUser();
            var token = sut.GenerateRefreshToken(true);

            // Act
            var newTokenResult = sut.Refresh(token, user);

            // Assert
            newTokenResult.Succeeded.Should().BeTrue();
            newTokenResult.Data
                .Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Refresh_ShouldNotRefreshInvalidToken()
        {
            // Arrange
            var sut = MakeSut();
            var settings = MakeSutSettings();
            var user = MakeUser();
            var token = new RefreshToken(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow.AddMinutes(-1),
                _dateTimeProvider);

            // Act
            var newTokenResult = sut.Refresh(token, user);

            // Assert
            newTokenResult.Failed.Should().BeTrue();
            newTokenResult.Data
                .Should().Be(default(string));
        }

        [Fact]
        public void IsAccessTokenValid_ShouldReturnTrueForValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var settings = MakeSutSettings();
            var user = MakeUser();
            var token = sut.GenerateAccessToken(user);

            // Act
            var isValidResult = sut.IsAccessTokenValid(token);

            // Assert
            isValidResult.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void IsAccessTokenValid_ShouldReturnFalseForInValidToken()
        {
            // Arrange
            var sut = MakeSut();
            var token = "somejwttoken";

            // Act
            var isValidResult = sut.IsAccessTokenValid(token);

            // Assert
            isValidResult.Succeeded.Should().BeFalse();
        }

        [Fact]
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

        [Fact]
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
    }
}