namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;

public class UserTypeDbModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserDbModel> Users { get; set; } = new();
}
