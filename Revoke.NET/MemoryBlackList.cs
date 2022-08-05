using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Revoke.NET
{
    public class MemoryCacheBlackList : IBlackList
    {
        private IMemoryCache _memoryCache;
        private TimeSpan? _defaultTtl;
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        public MemoryCacheBlackList(IMemoryCache memoryCache, TimeSpan? defaultTtl = null)
        {
            _defaultTtl = defaultTtl;
            _memoryCache = memoryCache;
        }

        public Task<bool> Revoke(string key)
        {
            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
                .SetAbsoluteExpiration(_defaultTtl ?? TimeSpan.MaxValue);
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            try
            {
                _memoryCache.Set(key, key, options);
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
                _memoryCache.Remove(key);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

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


        public Task<bool> IsRevoked(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal)
                .SetAbsoluteExpiration(expireAfter);
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            try
            {
                _memoryCache.Set(key, key, options);
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
                _memoryCache.Set(key, key, options);
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
        private ConcurrentDictionary<string, DateTime> _blackList;
        private readonly TimeSpan? defaultTtl;

        private MemoryBlackList(TimeSpan? defaultTtl)
        {
            this.defaultTtl = defaultTtl;
            _blackList = new ConcurrentDictionary<string, DateTime>();
        }

        public static MemoryBlackList CreateStore(TimeSpan? defaultExpirationDuration = null)
        {
            return new MemoryBlackList(defaultExpirationDuration);
        }

        public Task<bool> Delete(string key)
        {
            return Task.FromResult(_blackList.TryRemove(key, out _));
        }

        public Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            return Task.FromResult(_blackList.TryAdd(key, DateTime.Now.Add(expireAfter)));
        }

        public Task<bool> Revoke(string key, DateTime expireOn)
        {
            if (expireOn < DateTimeOffset.Now) return Task.FromResult(false);
            return Task.FromResult(_blackList.TryAdd(key, expireOn));
        }

        public Task DeleteAll()
        {
            _blackList.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> IsRevoked(string key)
        {
            if (_blackList.TryGetValue(key, out var item))
            {
                return Task.FromResult(item >= DateTime.Now);
            }

            return Task.FromResult(false);
        }

        public Task<bool> Revoke(string key)
        {
            if (DateTime.Now.Add(defaultTtl ?? TimeSpan.MaxValue) < DateTime.Now)
                return Task.FromResult(false);
            return Task.FromResult(_blackList.TryAdd(key, DateTime.Now.Add(defaultTtl ?? TimeSpan.MaxValue)));
        }
    }
}