namespace CardsAgainstMySanity.SharedKernel.Data;

public interface IRepository<T, TId> : IUnitOfWork, IDisposable
{
    Task<T> FindByIdAsync(TId id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
}
