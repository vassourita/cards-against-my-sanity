namespace CardsAgainstMySanity.Domain.Auth.Repositories;

using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.SharedKernel.Data;

public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{
}
