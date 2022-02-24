namespace CardsAgainstMySanity.Test.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAgainstMySanity.Domain.Auth.Repositories;
using CardsAgainstMySanity.Domain.Auth.Tokens;

public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _refreshTokens = new();

    public Task<RefreshToken> AddAsync(RefreshToken entity)
    {
        this._refreshTokens.Add(entity);
        return Task.FromResult(entity);
    }

    public Task CommitAsync() => Task.CompletedTask;

    public Task<int> CountAsync() => Task.FromResult(this._refreshTokens.Count);

    public Task DeleteAsync(RefreshToken entity)
    {
        this._refreshTokens.Remove(entity);
        return Task.CompletedTask;
    }

    public void Dispose()
    {

    }

    public Task<RefreshToken> FindByIdAsync(Guid id)
    {
        var refreshToken = this._refreshTokens.FirstOrDefault(x => x.Token == id);
        return Task.FromResult(refreshToken);
    }

    public Task RollbackAsync() => Task.CompletedTask;

    public Task UpdateAsync(RefreshToken entity)
    {
        var index = this._refreshTokens.FindIndex(x => x.Token == entity.Token);
        this._refreshTokens[index] = entity;
        return Task.CompletedTask;
    }
}
