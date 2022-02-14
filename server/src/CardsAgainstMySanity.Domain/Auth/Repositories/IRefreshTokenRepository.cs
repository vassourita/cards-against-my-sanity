using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.SharedKernel.Data;

namespace CardsAgainstMySanity.Domain.Auth.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
    {
    }
}