namespace Revoke.NET.EasyCaching;

using System;
using System.Threading.Tasks;
using global::EasyCaching.Core;

internal class EasyCachingBlackList : IBlackList
{
    private readonly TimeSpan? _defaultTtl;
    private readonly IEasyCachingProvider _easyCaching;

    public EasyCachingBlackList(
        IEasyCachingProvider easyCaching,
        TimeSpan? defaultTtl = null)
    {
        this._defaultTtl = defaultTtl;
        this._easyCaching = easyCaching;
    }

    public async Task<bool> Revoke(
        string key,
        TimeSpan expireAfter)
    {
        try
        {
            await this._easyCaching.SetAsync(key, key, expireAfter);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Revoke(
        string key,
        DateTime expireOn)
    {
        try
        {
            await this._easyCaching.SetAsync(key, key, expireOn - DateTime.Now);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Revoke(string key)
    {
        try
        {
            await this._easyCaching.SetAsync(key, key, this._defaultTtl ?? TimeSpan.MaxValue);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Delete(string key)
    {
        try
        {
            await this._easyCaching.RemoveAsync(key);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteAll()
    {
        try
        {
            await this._easyCaching.FlushAsync();
        }
        catch
        {
            // ignored
        }
    }

    public async Task<bool> IsRevoked(string key)
    {
        try
        {
            return await this._easyCaching.ExistsAsync(key);
        }
        catch
        {
            return false;
        }
    }
}