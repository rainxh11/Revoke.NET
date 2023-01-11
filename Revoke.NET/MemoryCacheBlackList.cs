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

    public MemoryCacheBlackList(
        IMemoryCache memoryCache,
        TimeSpan? defaultTtl = null)
    {
        this._defaultTtl = defaultTtl;
        this._memoryCache = memoryCache;
    }
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key,CancellationToken cancellationToken = default) instead.", false)]
    public Task<bool> Revoke(string key)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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
    
    public Task<bool> RevokeAsync(string key, CancellationToken cancellationToken = default)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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
    
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAsync(string key,CancellationToken cancellationToken = default) instead.", false)]
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
    
    public Task<bool> DeleteAsync(string key,CancellationToken cancellationToken = default)
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
    
    
    public Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested &&
            _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }

        _resetCacheToken = new CancellationTokenSource();

        return Task.CompletedTask;
    }
    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> DeleteAllAsync(CancellationToken cancellationToken = default) instead.", false)]
    public Task DeleteAll()
    {
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested &&
            _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }

        _resetCacheToken = new CancellationTokenSource();

        return Task.CompletedTask;
    }

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> IsRevokedAsync(string key, CancellationToken cancellationToken = default) instead.", false)]
    public Task<bool> IsRevoked(string key)
    {
        return Task.FromResult(this._memoryCache.TryGetValue(key, out _));
    }
    public Task<bool> IsRevokedAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(this._memoryCache.TryGetValue(key, out _));
    }

    public Task<bool> RevokeAsync(
        string key,
        TimeSpan expireAfter, CancellationToken cancellationToken = default)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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


    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key,TimeSpan expireOn, CancellationToken cancellationToken = default) instead.", false)]
    public Task<bool> Revoke(
        string key,
        TimeSpan expireAfter)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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
    
    public Task<bool> RevokeAsync(
        string key,
        DateTime expireOn, CancellationToken cancellationToken = default)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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

    [Obsolete(
        "This method is obsolete. Later versions of Revoke.NET will utilize the `Async` suffix on async methods with the addition of cancellation token parameters.  Please use >> RevokeAsync(string key,DateTime expireOn, CancellationToken cancellationToken = default) instead.", false)]
    public Task<bool> Revoke(
        string key,
        DateTime expireOn)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
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