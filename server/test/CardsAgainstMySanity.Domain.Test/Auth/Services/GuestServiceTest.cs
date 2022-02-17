using System;
using System.Security.Claims;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Test.Auth.Assets;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.SharedKernel;
using CardsAgainstMySanity.Domain.Auth;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class GuestServiceTest
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        public GuestServiceTest()
        {
            _dateTimeProvider = new FakeDateTimeProvider(DateTime.Now, DateTime.UtcNow);
        }
        // factories 
        private GuestService MakeSut()
            => new(
                makeFakeGuestRepository(),
                makeFakeTokenService(),
                _dateTimeProvider);
        private IGuestRepository makeFakeGuestRepository()
            => new FakeGuestRepository();
        #region make FakeTokenService
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
        private TokenService makeFakeTokenService()
            => new(
                MakeSutSettings(),
                new FakeRefreshTokenRepository(),
                _dateTimeProvider);
        #endregion
        #region testCases
        [Fact]
        async public void InitSession_ValidDTO_ShouldSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username"},
                "100.100.100.100"
            );
            // Assert
            result.Succeeded.Should().BeTrue();
            result.Should().BeOfType<Result<Guest>>();
            result.Data.Should().NotBe(null).And.BeOfType<Guest>();
        }
        [Fact]
        async public void InitSession_InvalidUsername_ShouldNotSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = ""},
                "192.168.0.1"
            );
            // Assert
            result.Succeeded.Should().BeFalse();
            result.Should().BeOfType<Result<Guest>>();
            result.Data.Should().Be(null).And.BeOfType<Guest>();
        }
        [Fact]
        async public void InitSession_InvalidIpAdress_ShouldNotSucceed(){
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username"},
                "256.256.256.256"
            );
            // Assert
            result.Succeeded.Should().BeFalse();
            result.Should().BeOfType<Result<Guest>>();
            result.Data.Should().Be(null).And.BeOfType<Guest>();
        }
        [Fact]
        async public void InitSession_EmptyIpAdress_ShouldNotSucceed(){
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username"},
                ""
            );
            // Assert
            result.Succeeded.Should().BeFalse();
            result.Should().BeOfType<Result<Guest>>();
            result.Data.Should().Be(null).And.BeOfType<Guest>();
        }
        #endregion
    }
}