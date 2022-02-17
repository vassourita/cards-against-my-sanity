using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;

namespace CardsAgainstMySanity.Domain.Test.Auth.Assets
{
    class FakeGuestRepository : IGuestRepository
    {
        private readonly List<Guest> _guests = new List<Guest>();

        public Task<Guest> AddAsync(Guest entity)
        {
            _guests.Add(entity);
            return Task.FromResult<Guest>(entity);
        }

        public Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_guests.Count);
        }

        public Task DeleteAsync(Guest entity)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public Task<Guest> FindByIdAsync(Guid id)
        {
            var guest = _guests.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(guest);
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