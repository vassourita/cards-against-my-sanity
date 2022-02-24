namespace CardsAgainstMySanity.SharedKernel.Data;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
