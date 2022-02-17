using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Tokens;

namespace CardsAgainstMySanity.Domain.Test.Auth.Assets
{
    public class TestUser : IUser
    {
        public TestUser(Guid id, string username, string ipAddress, string avatarUrl)
        {
            Id = id;
            Username = username;
            IpAddress = ipAddress;
            AvatarUrl = avatarUrl;
            CreatedAt = DateTime.UtcNow;
            RefreshTokens = new Collection<RefreshToken>();
        }

        public Guid Id { get; private set; }

        public string AvatarUrl { get; private set; }

        public string Username { get; private set; }

        public string IpAddress { get; private set; }

        public string AccessToken { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime LastPong { get; private set; }

        public ICollection<RefreshToken> RefreshTokens { get; private set; }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }

        public void SetAccessToken(string accessToken, string ipAddress)
        {
            AccessToken = accessToken;
            IpAddress = ipAddress;
        }
    }
}