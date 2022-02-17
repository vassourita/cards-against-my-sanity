using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories
{
    public class EFRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _dbContext;
        private readonly DbSet<RefreshToken> _refreshTokens;

        public EFRefreshTokenRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            _refreshTokens = dbContext.Set<RefreshToken>();
        }

        public Task<RefreshToken> AddAsync(RefreshToken entity)
        {
            _refreshTokens.Add(entity);

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(RefreshToken entity)
        {
            _refreshTokens.Remove(entity);

            return Task.CompletedTask;
        }

        public async Task<RefreshToken> FindByIdAsync(Guid id)
        {
            return await _refreshTokens.FirstOrDefaultAsync(t => t.Token == id);
        }

        public async Task<int> CountAsync()
        {
            return await _refreshTokens.CountAsync();
        }

        public Task UpdateAsync(RefreshToken entity)
        {
            _refreshTokens.Update(entity);

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