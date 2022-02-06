using CardsAgainstMySanity.SharedKernel.Data;

namespace CardsAgainstMySanity.Domain.Auth.Repositories
{
    public interface IGuestRepository : IRepository<Guest, Guid>
    {
    }
}