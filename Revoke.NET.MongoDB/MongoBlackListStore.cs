using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Revoke.NET;

namespace Revoke.NET.MongoDB
{
    public class MongoBlackListStore : IBlackListStore
    {
        private readonly IMongoCollection<BlackListItem> blacklist;

        private MongoBlackListStore(IMongoCollection<BlackListItem> blacklist)
        {
            this.blacklist = blacklist;
        }

        public static async Task<IBlackListStore> CreateStoreAsync(string dbName,
            MongoClientSettings clientSettings)
        {
            var client = new MongoClient(clientSettings);

            var db = client.GetDatabase(dbName);

            var keyIndex = Builders<BlackListItem>.IndexKeys.Ascending(x => x.Key);
            var ttlIndex = Builders<BlackListItem>.IndexKeys.Ascending(x => x.ExpireOn);

            var collection = db.GetCollection<BlackListItem>(nameof(BlackListItem));

            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<BlackListItem>(keyIndex, new CreateIndexOptions() { Unique = true }));
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<BlackListItem>(ttlIndex,
                    new CreateIndexOptions() { ExpireAfter = TimeSpan.FromMinutes(1) }));

            return new MongoBlackListStore(collection);
        }

        public async Task<bool> Delete(string key)
        {
            var result = await blacklist.DeleteOneAsync(x => x.Key == key);
            return result.IsAcknowledged;
        }

        public async Task DeleteAll()
        {
            await blacklist.DeleteManyAsync(x => true);
        }


        public async Task DeleteExpired()
        {
            await blacklist.DeleteManyAsync(x => x.ExpireOn < DateTimeOffset.Now);
        }

        public async Task<T> Get<T>(string key) where T : IBlackListItem
        {
            return await blacklist.Database.GetCollection<T>(nameof(IBlackListItem)).Find(x => x.Key == key)
                .FirstAsync();
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : IBlackListItem
        {
            return await blacklist.Database.GetCollection<T>(nameof(IBlackListItem)).Find(x => true).ToListAsync();
        }

        public async Task<bool> IsRevoked(string key)
        {
            var result = await blacklist.Find(x => x.Key == key).CountDocumentsAsync();
            return result > 0;
        }

        public async Task<bool> Revoke(string key)
        {
            try
            {
                await blacklist.InsertOneAsync(new BlackListItem(key, DateTimeOffset.MaxValue));
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
                await blacklist.InsertOneAsync(new BlackListItem(key, DateTimeOffset.Now.Add(expireAfter)));
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
                await blacklist.InsertOneAsync(new BlackListItem(key, expireOn));
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
                await blacklist.Database
                    .GetCollection<T>(nameof(IBlackListItem))
                    .InsertOneAsync(item);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}