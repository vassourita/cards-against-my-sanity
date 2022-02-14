using CardsAgainstMySanity.Domain.Auth;

namespace CardsAgainstMySanity.Presentation.Auth
{
    public class GuestViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        protected GuestViewModel()
        {
        }

        public static GuestViewModel FromGuest(Guest guest)
        {
            return new GuestViewModel
            {
                Id = guest.Id,
                Username = guest.Username,
                AvatarUrl = guest.AvatarUrl,
                CreatedAt = guest.CreatedAt
            };
        }
    }
}