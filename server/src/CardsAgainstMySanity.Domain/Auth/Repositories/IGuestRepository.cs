using CardsAgainstMySanity.SharedKernel.Data;

namespace CardsAgainstMySanity.Domain.Auth.Repositories
{
    public interface IGuestRepository : IRepository<Guest, Guid>
    {
        Task<Guest> GetByAccessTokenAsync(string token);
    }
}