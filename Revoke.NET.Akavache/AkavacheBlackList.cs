namespace Revoke.NET.Akavache;

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using global::Akavache;

public class AkavacheBlackList : IBlackList
{
    private static TimeSpan? _defaultTtl;
    private readonly IBlobCache _blackList;

    private AkavacheBlackList(IBlobCache blobcache)
    {
        this._blackList = blobcache;
    }

    public async Task<bool> Delete(string key)
    {
        try
        {
            await this._blackList.Invalidate(key);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteAll()
    {
        await this._blackList.InvalidateAll();
    }

    public async Task<bool> IsRevoked(string key)
    {
        try
        {
            await this._blackList.Vacuum();
            byte[] exist = await this._blackList.Get(key);

            return exist.Length > 0;
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
            await this._blackList.InsertObject(key, key, DateTimeOffset.Now.Add(_defaultTtl ?? TimeSpan.MaxValue));

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Revoke(
        string key,
        TimeSpan expireAfter)
    {
        try
        {
            await this._blackList.InsertObject(key, key, DateTimeOffset.Now.Add(expireAfter));

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
            await this._blackList.InsertObject(key, key, expireOn);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<IBlackList> CreateStoreAsync(
        string cacheName,
        IBlobCache blobCache,
        TimeSpan? defaultTtl = null)
    {
        _defaultTtl = defaultTtl;
        Registrations.Start(cacheName);
        await blobCache.Vacuum();

        return new AkavacheBlackList(blobCache);
    }
}