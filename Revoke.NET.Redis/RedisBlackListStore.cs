using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;

namespace Revoke.NET.Redis
{
    public class RedisBlackListStore : IBlackListStore
    {
        private IDatabase blacklist;
        private IEnumerable<IServer> servers;

        private RedisBlackListStore(IDatabase blacklist, IEnumerable<IServer> servers)
        {
            this.blacklist = blacklist;
            this.servers = servers;
        }

        public static async Task<IBlackListStore> CreateStoreAsync(string connectionString)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AllowAdmin = true;
            var redis = await ConnectionMultiplexer.ConnectAsync(options);
            var blacklist = redis.GetDatabase();
            var servers = redis.GetEndPoints().Select(x => redis.GetServer(x));

            return new RedisBlackListStore(blacklist, servers);
        }

        public async Task<bool> Delete(string key)
        {
            return await blacklist.KeyDeleteAsync(key);
        }

        public async Task DeleteAll()
        {
            foreach (var key in servers.SelectMany(x => x.Keys()))
            {
                await blacklist.KeyDeleteAsync(key);
            }
        }


        public Task DeleteExpired()
        {
            return Task.CompletedTask;
        }

        public async Task<T> Get<T>(string key) where T : IBlackListItem
        {
            var value = await blacklist.StringGetAsync(key);
            return JsonSerializer.Deserialize<T>((string)value.Box());
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : IBlackListItem
        {
            var temp = new List<T>();
            foreach (var key in servers.SelectMany(x => x.Keys()))
            {
                var value = await blacklist.StringGetAsync(key);
                temp.Add(JsonSerializer.Deserialize<T>((string)value.Box()));
            }

            return temp;
        }

        public async Task<bool> IsRevoked(string key)
        {
            var value = await blacklist.StringGetAsync(key);
            return !value.HasValue;
        }

        public async Task<bool> Revoke(string key)
        {
            var value = await blacklist.StringSetAndGetAsync(key,
                JsonSerializer.Serialize(new BlackListItem(key, DateTimeOffset.MaxValue)));
            return value.HasValue;
        }

        public async Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            var value = await blacklist.StringSetAndGetAsync(key,
                JsonSerializer.Serialize(new BlackListItem(key, DateTimeOffset.MaxValue)), expireAfter);
            return value.HasValue;
        }

        public async Task<bool> Revoke(string key, DateTimeOffset expireOn)
        {
            var value = await blacklist.StringSetAndGetAsync(key,
                JsonSerializer.Serialize(new BlackListItem(key, DateTimeOffset.MaxValue)),
                expireOn - DateTimeOffset.Now);
            return value.HasValue;
        }

        public async Task<bool> Revoke<T>(T item) where T : IBlackListItem
        {
            var value = await blacklist.StringSetAndGetAsync(item.Key, JsonSerializer.Serialize(item));
            return value.HasValue;
        }
    }
}