namespace CardsAgainstMySanity.Test.Assets;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Tokens;

public class TestUser : IUser
{
    public TestUser(Guid id, string username, string ipAddress, string avatarUrl)
    {
        this.Id = id;
        this.Username = username;
        this.IpAddress = ipAddress;
        this.AvatarUrl = avatarUrl;
        this.CreatedAt = DateTime.UtcNow;
        this.RefreshTokens = new Collection<RefreshToken>();
    }

    public Guid Id { get; private set; }

    public string AvatarUrl { get; private set; }

    public string Username { get; private set; }

    public string IpAddress { get; private set; }

    public string AccessToken { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastPong { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; }

    public void AddRefreshToken(RefreshToken refreshToken) => this.RefreshTokens.Add(refreshToken);

    public void SetAccessToken(string accessToken, string ipAddress)
    {
        this.AccessToken = accessToken;
        this.IpAddress = ipAddress;
    }
}
