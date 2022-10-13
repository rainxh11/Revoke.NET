namespace Revoke.NET;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MemoryBlackList : IBlackList
{
    private readonly ConcurrentDictionary<string, DateTime> _blackList;
    private readonly TimeSpan? _defaultTtl;

    private MemoryBlackList(TimeSpan? defaultTtl)
    {
        this._defaultTtl = defaultTtl;
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
        if (DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue) < DateTime.Now)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(this._blackList.TryAdd(key, DateTime.Now.Add(this._defaultTtl ?? TimeSpan.MaxValue)));
    }

    public static MemoryBlackList CreateStore(TimeSpan? defaultExpirationDuration = null)
    {
        return new MemoryBlackList(defaultExpirationDuration);
    }
}