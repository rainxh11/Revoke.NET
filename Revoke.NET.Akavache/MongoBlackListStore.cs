using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Revoke.NET;
using Akavache;
using Akavache.Sqlite3;

namespace Revoke.NET
{
    public class AkavacheBlackListStore : IBlackListStore
    {
        private readonly IBlobCache blacklist;

        private AkavacheBlackListStore(IBlobCache blobcache)
        {
            this.blacklist = blobcache;
        }

        public static async Task<IBlackListStore> CreateStoreAsync(string cacheName, IBlobCache blobCache)
        {
            Akavache.Registrations.Start(cacheName);
            await blobCache.Vacuum();
            return new AkavacheBlackListStore(blobCache);
        }

        public async Task<bool> Delete(string key)
        {
            try
            {
                await blacklist.Invalidate(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteAll()
        {
            await blacklist.InvalidateAll();
        }


        public async Task DeleteExpired()
        {
            await blacklist.Vacuum();
            ;
        }

        public async Task<T> Get<T>(string key) where T : IBlackListItem
        {
            return await blacklist.GetObject<T>(key);
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : IBlackListItem
        {
            return await blacklist.GetAllObjects<T>();
        }

        public async Task<bool> IsRevoked(string key)
        {
            try
            {
                await blacklist.Vacuum();
                var exist = await blacklist.Get(key);
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
                await blacklist.InsertObject(key, new BlackListItem(key, DateTimeOffset.MaxValue),
                    DateTimeOffset.MaxValue);
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
                await blacklist.InsertObject(key, new BlackListItem(key, DateTimeOffset.Now.Add(expireAfter)),
                    expireAfter);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Revoke(string key, DateTimeOffset expireOn)
        {
            try
            {
                await blacklist.InsertObject(key, new BlackListItem(key, expireOn), expireOn);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Revoke<T>(T item) where T : IBlackListItem
        {
            try
            {
                await blacklist.InsertObject(item.Key, item, item.ExpireOn);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}