namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories;

using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.EntityFrameworkCore;

public class EFRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DataContext _dbContext;
    private readonly DbSet<RefreshToken> _refreshTokens;

    public EFRefreshTokenRepository(DataContext dbContext)
    {
        this._dbContext = dbContext;
        this._refreshTokens = dbContext.Set<RefreshToken>();
    }

    public Task<RefreshToken> AddAsync(RefreshToken entity)
    {
        this._refreshTokens.Add(entity);

        return Task.FromResult(entity);
    }

    public Task DeleteAsync(RefreshToken entity)
    {
        this._refreshTokens.Remove(entity);

        return Task.CompletedTask;
    }

    public async Task<RefreshToken> FindByIdAsync(Guid id) => await this._refreshTokens.FirstOrDefaultAsync(t => t.Token == id);

    public async Task<int> CountAsync() => await this._refreshTokens.CountAsync();

    public Task UpdateAsync(RefreshToken entity)
    {
        this._refreshTokens.Update(entity);

        return Task.FromResult(entity);
    }

    public Task CommitAsync() => this._dbContext.SaveChangesAsync();

    public Task RollbackAsync() => Task.CompletedTask;

    public void Dispose() => this._dbContext.Dispose();
}
