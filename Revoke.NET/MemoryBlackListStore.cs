using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revoke.NET
{
    public class MemoryBlackListStore : IBlackListStore
    {
        private ConcurrentDictionary<string, IBlackListItem> blackList;
#nullable enable
        private TimeSpan? defaultTtl;
#nullable disable

        private MemoryBlackListStore(TimeSpan? defaultTtl)
        {
            this.defaultTtl = defaultTtl;
            blackList = new ConcurrentDictionary<string, IBlackListItem>();
        }

        public static MemoryBlackListStore CreateStore(TimeSpan? defaultExpirationDuration = null)
        {
            return new MemoryBlackListStore(defaultExpirationDuration);
        }

        public Task<bool> Delete(string key)
        {
            return Task.FromResult(blackList.TryRemove(key, out _));
        }

        public Task DeleteAll()
        {
            blackList.Clear();
            return Task.CompletedTask;
        }

        public Task DeleteExpired()
        {
            blackList = new ConcurrentDictionary<string, IBlackListItem>(
                blackList.Where(x => DateTime.Now > x.Value.ExpireOn));
            return Task.CompletedTask;
        }

        public Task<T> Get<T>(string key) where T : IBlackListItem
        {
            if (blackList.TryGetValue(key, out var item))
            {
                return Task.FromResult((T)item);
            }

            return null;
        }

        public Task<IEnumerable<T>> GetAll<T>() where T : IBlackListItem
        {
            var items = blackList.Select(item => item.Value);
            return Task.FromResult(items.Cast<T>());
        }

        public Task<bool> IsRevoked(string key)
        {
            if (blackList.TryGetValue(key, out var item))
            {
                return Task.FromResult(item.ExpireOn <= DateTimeOffset.Now);
            }

            return Task.FromResult(false);
        }

        public Task<bool> Revoke(string key)
        {
            var item = new BlackListItem(key,
                defaultTtl is null ? DateTimeOffset.Now.Add(defaultTtl ?? TimeSpan.Zero) : DateTimeOffset.MaxValue);

            if (blackList.ContainsKey(key))
            {
                return Task.FromResult(blackList.TryUpdate(key, item, item));
            }
            else
            {
                return Task.FromResult(blackList.TryAdd(key, item));
            }
        }

        public Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            var item = new BlackListItem(key, DateTimeOffset.Now.Add(defaultTtl ?? expireAfter));

            if (blackList.ContainsKey(key))
            {
                return Task.FromResult(blackList.TryUpdate(key, item, item));
            }
            else
            {
                return Task.FromResult(blackList.TryAdd(key, item));
            }
        }

        public Task<bool> Revoke(string key, DateTimeOffset expireOn)
        {
            var item = new BlackListItem(key,
                defaultTtl is null ? DateTimeOffset.Now.Add(defaultTtl ?? TimeSpan.Zero) : expireOn);

            if (blackList.ContainsKey(key))
            {
                return Task.FromResult(blackList.TryUpdate(key, item, item));
            }
            else
            {
                return Task.FromResult(blackList.TryAdd(key, item));
            }
        }
    }
}