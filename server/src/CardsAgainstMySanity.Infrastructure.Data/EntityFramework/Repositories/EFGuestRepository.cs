namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories;

using AutoMapper;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

public class EFGuestRepository : IGuestRepository
{
    private readonly DataContext _dbContext;
    private readonly IMapper _mapper;
    private readonly DbSet<UserDbModel> _users;

    public EFGuestRepository(DataContext dbContext, IMapper mapper)
    {
        this._dbContext = dbContext;
        this._mapper = mapper;
        this._users = dbContext.Set<UserDbModel>();
    }

    private IQueryable<UserDbModel> _guests => this._users.Where(u => u.UserTypeId == 2);

    public Task<Guest> AddAsync(Guest entity)
    {
        var user = this._mapper.Map<UserDbModel>(entity);
        this._users.Add(user);

        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Guest entity)
    {
        var user = this._users.Find(entity.Id);
        this._users.Remove(user);

        return Task.CompletedTask;
    }

    public async Task<Guest> FindByIdAsync(Guid id)
    {
        var user = await this._guests.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return null;
        }

        var guest = this._mapper.Map<Guest>(user);

        return guest;
    }

    public async Task<int> CountAsync() => await this._guests.CountAsync();

    public Task UpdateAsync(Guest entity)
    {
        var user = this._mapper.Map<UserDbModel>(entity);
        this._users.Update(user);

        return Task.FromResult(entity);
    }

    public Task CommitAsync() => this._dbContext.SaveChangesAsync();

    public Task RollbackAsync() => Task.CompletedTask;

    public void Dispose() => this._dbContext.Dispose();
}
