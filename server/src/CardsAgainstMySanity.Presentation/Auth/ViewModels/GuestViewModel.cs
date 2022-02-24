namespace CardsAgainstMySanity.Presentation.Auth;

using CardsAgainstMySanity.Domain.Auth;

public class GuestViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    protected GuestViewModel()
    {
    }

    public static GuestViewModel FromGuest(Guest guest) => new()
    {
        Id = guest.Id,
        Username = guest.Username,
        AvatarUrl = guest.AvatarUrl,
        CreatedAt = guest.CreatedAt
    };
}
