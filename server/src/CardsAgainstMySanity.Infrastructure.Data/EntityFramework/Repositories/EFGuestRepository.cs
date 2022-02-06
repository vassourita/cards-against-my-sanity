using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories
{

    public class EFGuestRepository : GenericRepository<Guest, Guid>, IGuestRepository
    {
        private new readonly DataContext _dbContext;
        private readonly DbSet<Guest> _guests;

        public EFGuestRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Guest> FindByIdAsync(Guid id)
        {
            return await _entities.Where(g => g.Id == id).SingleOrDefaultAsync();
        }
    }
}