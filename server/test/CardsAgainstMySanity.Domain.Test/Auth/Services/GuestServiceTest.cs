namespace CardsAgainstMySanity.Domain.Test.Auth.Services;

using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Domain.Providers;
using CardsAgainstMySanity.SharedKernel.Validation;
using CardsAgainstMySanity.Test;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class GuestServiceTest : TestBase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuestRepository _guestRepository;

    public GuestServiceTest() : base()
    {
        this._dateTimeProvider = this.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        this._guestRepository = this.ServiceProvider.GetRequiredService<IGuestRepository>();
    }

    private GuestService MakeSut()
        => new(
            this._guestRepository,
            this.ServiceProvider.GetRequiredService<TokenService>(),
            this._dateTimeProvider,
            this.ServiceProvider.GetRequiredService<IModelValidator<GuestInitSessionDto>>());

    #region testCases
    [Fact]
    public async void InitSessionValidDTOShouldSucceed()
    {
        // Arrange
        var sut = this.MakeSut();

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

        var dbResult = await this._guestRepository.FindByIdAsync(result.Data.Id);
        dbResult.Should().NotBeNull();
    }

    [Fact]
    public async void InitSessionInvalidUsernameShouldNotSucceed()
    {
        // Arrange
        var sut = this.MakeSut();

        // Act
        var result = await sut.InitSession(
            new GuestInitSessionDto() { Username = "", IpAddress = "100.100.100.100" }
        );

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Data.Should().Be(null);
        result.Error.Should().NotBeNull()
            .And.HaveCount(1);

        var dbResult = await this._guestRepository.CountAsync();
        dbResult.Should().Be(0);
    }

    [Fact]
    public async void InitSessionInvalidIpAddressShouldNotSucceed()
    {
        // Arrange
        var sut = this.MakeSut();

        // Act
        var result = await sut.InitSession(
            new GuestInitSessionDto() { Username = "test-username", IpAddress = "test-ip" }
        );

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Data.Should().Be(null);
        result.Error.Should().NotBeNull()
            .And.HaveCount(1);

        var dbResult = await this._guestRepository.CountAsync();
        dbResult.Should().Be(0);
    }

    [Fact]
    public async void InitSessionEmptyIpAddressShouldNotSucceed()
    {
        // Arrange
        var sut = this.MakeSut();

        // Act
        var result = await sut.InitSession(
            new GuestInitSessionDto() { Username = "test-username", IpAddress = "" }
        );

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Data.Should().Be(null);
        result.Error.Should().NotBeNull()
            .And.HaveCount(1);

        var dbResult = await this._guestRepository.CountAsync();
        dbResult.Should().Be(0);
    }
    #endregion testCases
}
