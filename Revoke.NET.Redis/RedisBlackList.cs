namespace Revoke.NET.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

public class RedisBlackList : IBlackList
{
    private static TimeSpan? _defaultTtl;
    private readonly IDatabase _blackList;
    private readonly IEnumerable<IServer> _servers;

    private RedisBlackList(IDatabase blackList, IEnumerable<IServer> servers)
    {
        this._blackList = blackList;
        this._servers = servers;
    }

    public async Task<bool> Delete(string key)
    {
        return await this._blackList.KeyDeleteAsync(key);
    }

    public async Task DeleteAll()
    {
        foreach (var key in this._servers.SelectMany(x => x.Keys()))
        {
            await this._blackList.KeyDeleteAsync(key);
        }
    }

    public async Task<bool> IsRevoked(string key)
    {
        var value = await this._blackList.StringGetAsync(key);

        return !value.HasValue;
    }

    public async Task<bool> Revoke(string key)
    {
        var value = await this._blackList.StringSetAndGetAsync(key, key, _defaultTtl ?? TimeSpan.MaxValue);

        return value.HasValue;
    }

    public async Task<bool> Revoke(string key, TimeSpan expireAfter)
    {
        var value = await this._blackList.StringSetAndGetAsync(key, key, expireAfter);

        return value.HasValue;
    }

    public async Task<bool> Revoke(string key, DateTime expireOn)
    {
        var value = await this._blackList.StringSetAndGetAsync(key, key, expireOn - DateTimeOffset.Now);

        return value.HasValue;
    }

    public static async Task<IBlackList> CreateStoreAsync(string connectionString, TimeSpan? defaultTtl = null)
    {
        _defaultTtl = defaultTtl;
        var options = ConfigurationOptions.Parse(connectionString);
        options.AllowAdmin = true;
        var redis = await ConnectionMultiplexer.ConnectAsync(options);
        var blacklist = redis.GetDatabase();
        var servers = redis.GetEndPoints()
            .Select(x => redis.GetServer(x));

        return new RedisBlackList(blacklist, servers);
    }
}