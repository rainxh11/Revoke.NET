namespace Revoke.NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

public class MemoryCacheBlackList : IBlackList
{
    private static CancellationTokenSource _resetCacheToken = new();
    private readonly TimeSpan? _defaultTtl;
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheBlackList(IMemoryCache memoryCache, TimeSpan? defaultTtl = null)
    {
        this._defaultTtl = defaultTtl;
        this._memoryCache = memoryCache;
    }

    public Task<bool> Revoke(string key)
    {
        var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
            .SetAbsoluteExpiration(this._defaultTtl ?? TimeSpan.MaxValue);
        options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
        try
        {
            this._memoryCache.Set(key, key, options);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> Delete(string key)
    {
        try
        {
            this._memoryCache.Remove(key);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task DeleteAll()
    {
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }

        _resetCacheToken = new CancellationTokenSource();

        return Task.CompletedTask;
    }

    public Task<bool> IsRevoked(string key)
    {
        return Task.FromResult(this._memoryCache.TryGetValue(key, out _));
    }

    public Task<bool> Revoke(string key, TimeSpan expireAfter)
    {
        var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
            .SetAbsoluteExpiration(expireAfter);
        options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
        try
        {
            this._memoryCache.Set(key, key, options);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> Revoke(string key, DateTime expireOn)
    {
        var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
            .SetAbsoluteExpiration(expireOn);
        options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        try
        {
            this._memoryCache.Set(key, key, options);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}