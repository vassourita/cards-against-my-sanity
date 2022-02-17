using System;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;

namespace CardsAgainstMySanity.Domain.Test.Auth.Assets {
    class FakeGuestRepository : IGuestRepository
    {
        public Task<Guest> AddAsync(Guest entity)
        {
            return Task.FromResult<Guest>(entity);
        }

        public Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guest entity)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            return;
        }

        public Task<Guest> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Guest entity)
        {
            throw new NotImplementedException();
        }
    }
}