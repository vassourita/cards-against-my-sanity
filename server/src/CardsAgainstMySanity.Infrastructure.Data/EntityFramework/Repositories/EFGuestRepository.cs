using AutoMapper;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories
{

    public class EFGuestRepository : IGuestRepository
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DbSet<UserDbModel> _users;

        public EFGuestRepository(DataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _users = dbContext.Set<UserDbModel>();
        }

        private IQueryable<UserDbModel> _guests => _users.Where(u => u.UserTypeId == 2);

        public Task<Guest> AddAsync(Guest entity)
        {
            var user = _mapper.Map<UserDbModel>(entity);
            _users.Add(user);

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guest entity)
        {
            var user = _users.Find(entity.Id);
            _users.Remove(user);

            return Task.CompletedTask;
        }

        public async Task<Guest> FindByIdAsync(Guid id)
        {
            var user = await _guests.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return null;

            var guest = _mapper.Map<Guest>(user);

            return guest;
        }

        public async Task<int> CountAsync()
        {
            return await _guests.CountAsync();
        }

        public Task UpdateAsync(Guest entity)
        {
            var user = _mapper.Map<UserDbModel>(entity);
            _users.Update(user);

            return Task.FromResult(entity);
        }

        public Task CommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}