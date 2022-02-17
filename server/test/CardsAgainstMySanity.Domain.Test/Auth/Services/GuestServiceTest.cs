using System;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Domain.Test.Auth.Assets;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Infrastructure.Validators;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class GuestServiceTest
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuestRepository _fakeGuestRepository;

        public GuestServiceTest()
        {
            _dateTimeProvider = new FakeDateTimeProvider(DateTime.Now, DateTime.UtcNow);
            _fakeGuestRepository = MakeFakeGuestRepository();
        }

        #region factories
        private GuestService MakeSut()
            => new(
                _fakeGuestRepository,
                MakeTokenService(),
                _dateTimeProvider,
                new GuestInitSessionDtoValidationAdapter());

        private IGuestRepository MakeFakeGuestRepository()
            => new FakeGuestRepository();

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

        private TokenService MakeTokenService()
            => new(
                MakeSutSettings(),
                new FakeRefreshTokenRepository(),
                _dateTimeProvider);
        #endregion
        #region testCases
        [Fact]
        public async void InitSession_ValidDTO_ShouldSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username", IpAddress = "100.100.100.100" }
            );

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Error.Should().BeNull();
            result.Data.AccessToken.Should().NotBeNullOrEmpty();
            result.Data.RefreshTokens.Should().HaveCountGreaterThan(0);

            var dbResult = await _fakeGuestRepository.FindByIdAsync(result.Data.Id);
            dbResult.Should().NotBeNull();
        }

        [Fact]
        public async void InitSession_InvalidUsername_ShouldNotSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "", IpAddress = "100.100.100.100" }
            );

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Data.Should().Be(null);
            result.Error.Should().NotBeNull()
                .And.HaveCount(1);

            var dbResult = await _fakeGuestRepository.CountAsync();
            dbResult.Should().Be(0);
        }

        [Fact]
        public async void InitSession_InvalidIpAddress_ShouldNotSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username", IpAddress = "test-ip" }
            );

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Data.Should().Be(null);
            result.Error.Should().NotBeNull()
                .And.HaveCount(1);

            var dbResult = await _fakeGuestRepository.CountAsync();
            dbResult.Should().Be(0);
        }

        [Fact]
        public async void InitSession_EmptyIpAddress_ShouldNotSucceed()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var result = await sut.InitSession(
                new GuestInitSessionDto() { Username = "test-username", IpAddress = "" }
            );

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Data.Should().Be(null);
            result.Error.Should().NotBeNull()
                .And.HaveCount(1);

            var dbResult = await _fakeGuestRepository.CountAsync();
            dbResult.Should().Be(0);
        }
    }
    #endregion
}