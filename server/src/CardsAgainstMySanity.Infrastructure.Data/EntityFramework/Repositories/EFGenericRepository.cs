using CardsAgainstMySanity.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Repositories
{

    public abstract class GenericRepository<T, TKey> : IRepository<T, TKey>
        where T : class
    {
        protected readonly DataContext _dbContext;
        protected readonly DbSet<T> _entities;

        public GenericRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            _entities = dbContext.Set<T>();
        }

        public abstract Task<T> FindByIdAsync(TKey id);

        public Task<T> AddAsync(T entity)
        {
            _entities.Add(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            return Task.CompletedTask;
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