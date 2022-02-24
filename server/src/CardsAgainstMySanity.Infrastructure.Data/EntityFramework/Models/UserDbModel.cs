namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;

using CardsAgainstMySanity.Domain.Auth.Tokens;

public class UserDbModel
{
    public Guid Id { get; set; }
    public string AvatarUrl { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string IpAddress { get; set; }
    public string AccessToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastPong { get; set; }
    public UserTypeDbModel UserType { get; set; }
    public int UserTypeId { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
