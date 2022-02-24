namespace CardsAgainstMySanity.Domain.Auth;

using CardsAgainstMySanity.Domain.Auth.Tokens;

public interface IUser
{
    Guid Id { get; }
    string AvatarUrl { get; }
    string Username { get; }
    string IpAddress { get; }
    string AccessToken { get; }
    DateTime CreatedAt { get; }
    DateTime LastPong { get; }

    ICollection<RefreshToken> RefreshTokens { get; }

    void SetAccessToken(string accessToken, string ipAddress);
    void AddRefreshToken(RefreshToken refreshToken);
}
