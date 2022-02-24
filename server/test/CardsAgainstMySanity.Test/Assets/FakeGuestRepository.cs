namespace CardsAgainstMySanity.Test.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;

public class FakeGuestRepository : IGuestRepository
{
    private readonly List<Guest> _guests = new();

    public Task<Guest> AddAsync(Guest entity)
    {
        this._guests.Add(entity);
        return Task.FromResult(entity);
    }

    public Task CommitAsync() => Task.CompletedTask;

    public Task<int> CountAsync() => Task.FromResult(this._guests.Count);

    public Task DeleteAsync(Guest entity) => Task.CompletedTask;

    public void Dispose()
    {

    }

    public Task<Guest> FindByIdAsync(Guid id)
    {
        var guest = this._guests.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(guest);
    }

    public Task RollbackAsync() => Task.CompletedTask;

    public Task UpdateAsync(Guest entity) => throw new NotImplementedException();
}
