namespace CardsAgainstMySanity.Domain.Identity;

using CardsAgainstMySanity.SharedKernel.Data;

public interface IGuestRepository : IRepository<Guest, Guid>
{
    Task<Guest> FindByNameAsync(string name);
}