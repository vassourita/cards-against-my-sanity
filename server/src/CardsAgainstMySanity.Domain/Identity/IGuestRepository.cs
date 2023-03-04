namespace CardsAgainstMySanity.Domain.Identity;

using CardsAgainstMySanity.SharedKernel.Data;

public interface IGuestRepository : IRepository<Guest, Guid>
{
    Task<Guest> FindActiveByNameAsync(string name);
    Task DeactivateAsync(Guid name);
}
