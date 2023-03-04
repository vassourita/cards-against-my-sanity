namespace CardsAgainstMySanity.Presentation.Identity.ViewModels;

using CardsAgainstMySanity.Domain.Identity;

public class GuestViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public static GuestViewModel FromEntity(Guest guest) => new()
    {
        Id = guest.Id,
        Name = guest.Name
    };
}
