using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;

namespace Revoke.NET.EasyCaching;

internal class EasyCachingBlackList : IBlackList
{
    private readonly IEasyCachingProvider _easyCaching;
    private readonly TimeSpan? _defaultTtl;

    public EasyCachingBlackList(IEasyCachingProvider easyCaching, TimeSpan? defaultTtl = null)
    {
        _defaultTtl = defaultTtl;
        _easyCaching = easyCaching;
    }

    public async Task<bool> Revoke(string key, TimeSpan expireAfter)
    {
        try
        {
            await _easyCaching.SetAsync(key, key, expireAfter);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Revoke(string key, DateTime expireOn)
    {
        try
        {
            await _easyCaching.SetAsync(key, key, expireOn - DateTime.Now);
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
            await _easyCaching.SetAsync(key, key, _defaultTtl ?? TimeSpan.MaxValue);
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
            await _easyCaching.RemoveAsync(key);
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
            await _easyCaching.FlushAsync();
        }
        catch
        {
        }
    }

    public async Task<bool> IsRevoked(string key)
    {
        try
        {
            return await _easyCaching.ExistsAsync(key);
        }
        catch
        {
            return false;
        }
    }
}