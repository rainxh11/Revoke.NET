using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Registrations = Akavache.Registrations;

namespace Revoke.NET.Akavache
{
    public class AkavacheBlackList : IBlackList
    {
        private readonly IBlobCache _blackList;
        private static TimeSpan? _defaultTtl;

        private AkavacheBlackList(IBlobCache blobcache)
        {
            this._blackList = blobcache;
        }

        public static async Task<IBlackList> CreateStoreAsync(string cacheName, IBlobCache blobCache,
            TimeSpan? defaultTtl = null)
        {
            _defaultTtl = defaultTtl;
            Registrations.Start(cacheName);
            await blobCache.Vacuum();
            return new AkavacheBlackList(blobCache);
        }


        public async Task<bool> Delete(string key)
        {
            try
            {
                await _blackList.Invalidate(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteAll()
        {
            await _blackList.InvalidateAll();
        }

        public async Task<bool> IsRevoked(string key)
        {
            try
            {
                await _blackList.Vacuum();
                var exist = await _blackList.Get(key);
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
                await _blackList.InsertObject(key, key, DateTimeOffset.Now.Add(_defaultTtl ?? TimeSpan.MaxValue));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            try
            {
                await _blackList.InsertObject(key, key, DateTimeOffset.Now.Add(expireAfter));

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
                await _blackList.InsertObject(key, key, expireOn);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}