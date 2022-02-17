using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;

namespace CardsAgainstMySanity.Domain.Test.Auth.Assets
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
            return Task.FromResult(_refreshTokens.Find(x => x.Token == id));
        }

        public Task RollbackAsync()
        {
            _refreshTokens.Clear();
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