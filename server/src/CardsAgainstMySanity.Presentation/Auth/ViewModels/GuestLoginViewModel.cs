using CardsAgainstMySanity.Domain.Auth;

namespace CardsAgainstMySanity.Presentation.Auth
{
    public class GuestLoginViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string AvatarUrl { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }

        protected GuestLoginViewModel()
        {
        }

        public static GuestLoginViewModel FromGuest(Guest guest)
        {
            var refreshToken = guest.RefreshTokens.OrderBy(t => t.CreatedAt).Last().Token.ToString();
            return new GuestLoginViewModel
            {
                Id = guest.Id,
                Username = guest.Username,
                AccessToken = guest.AccessToken,
                AvatarUrl = guest.AvatarUrl,
                RefreshToken = refreshToken,
                CreatedAt = guest.CreatedAt
            };
        }
    }
}