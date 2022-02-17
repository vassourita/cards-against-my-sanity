using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using System.Linq;

namespace CardsAgainstMySanity.Test.Assets
{
    public class FakeRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

        public Task<RefreshToken> AddAsync(RefreshToken entity)
        {
            _refreshTokens.Add(entity);
            return Task.FromResult(entity);
        }

        public Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_refreshTokens.Count);
        }

        public Task DeleteAsync(RefreshToken entity)
        {
            _refreshTokens.Remove(entity);
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public Task<RefreshToken> FindByIdAsync(Guid id)
        {
            var refreshToken = _refreshTokens.FirstOrDefault(x => x.Token == id);
            return Task.FromResult(refreshToken);
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(RefreshToken entity)
        {
            var index = _refreshTokens.FindIndex(x => x.Token == entity.Token);
            _refreshTokens[index] = entity;
            return Task.CompletedTask;
        }
    }
}