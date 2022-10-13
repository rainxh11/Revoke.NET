namespace Revoke.NET;

using System;
using System.Collections.Concurrent;
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

public class MemoryBlackList : IBlackList
{
    private readonly TimeSpan? defaultTtl;
    private readonly ConcurrentDictionary<string, DateTime> _blackList;

    private MemoryBlackList(TimeSpan? defaultTtl)
    {
        this.defaultTtl = defaultTtl;
        this._blackList = new ConcurrentDictionary<string, DateTime>();
    }

    public Task<bool> Delete(string key)
    {
        return Task.FromResult(this._blackList.TryRemove(key, out _));
    }

    public Task<bool> Revoke(string key, TimeSpan expireAfter)
    {
        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(expireAfter)));
    }

    public Task<bool> Revoke(string key, DateTime expireOn)
    {
        if (expireOn < DateTimeOffset.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, expireOn));
    }

    public Task DeleteAll()
    {
        this._blackList.Clear();

        return Task.CompletedTask;
    }

    public Task<bool> IsRevoked(string key)
    {
        if (this._blackList.TryGetValue(key, out var item))
        {
            return Task.FromResult(item >= DateTime.Now);
        }

        return Task.FromResult(false);
    }

    public Task<bool> Revoke(string key)
    {
        if (DateTime.Now.Add(this.defaultTtl ?? TimeSpan.MaxValue) < DateTime.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(this.defaultTtl ?? TimeSpan.MaxValue)));
    }

    public static MemoryBlackList CreateStore(TimeSpan? defaultExpirationDuration = null)
    {
        return new MemoryBlackList(defaultExpirationDuration);
    }
}