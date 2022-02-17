using CardsAgainstMySanity.Domain.Auth.Services;
using Xunit;
using FluentAssertions;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Test;
using Microsoft.Extensions.DependencyInjection;
using CardsAgainstMySanity.SharedKernel.Validation;

namespace CardsAgainstMySanity.Domain.Test.Auth.Services
{
    public class GuestServiceTest : TestBase
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuestRepository _guestRepository;

        public GuestServiceTest() : base()
        {
            _dateTimeProvider = ServiceProvider.GetRequiredService<IDateTimeProvider>();
            _guestRepository = ServiceProvider.GetRequiredService<IGuestRepository>();
        }

        private GuestService MakeSut()
            => new(
                _guestRepository,
                ServiceProvider.GetRequiredService<TokenService>(),
                _dateTimeProvider,
                ServiceProvider.GetRequiredService<IModelValidator<GuestInitSessionDto>>());

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

            var dbResult = await _guestRepository.FindByIdAsync(result.Data.Id);
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

            var dbResult = await _guestRepository.CountAsync();
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

            var dbResult = await _guestRepository.CountAsync();
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

            var dbResult = await _guestRepository.CountAsync();
            dbResult.Should().Be(0);
        }
    }
    #endregion
}