using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Revoke.NET;

namespace Revoke.NET.MongoDB
{
    public class MongoBlackListItem
    {
        [BsonId] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public MongoBlackListItem(string key, DateTime expireOn)
        {
            Key = key;
            ExpireOn = expireOn;
        }

        public string Key { get; set; }
        public DateTime ExpireOn { get; set; }
    }

    public class MongoBlackList : IBlackList
    {
        private readonly IMongoCollection<MongoBlackListItem> _blacklist;

        private MongoBlackList(IMongoCollection<MongoBlackListItem> blacklist)
        {
            this._blacklist = blacklist;
        }

        public static async Task<IBlackList> CreateStoreAsync(string dbName,
            MongoClientSettings clientSettings)
        {
            var client = new MongoClient(clientSettings);

            var db = client.GetDatabase(dbName);

            var keyIndex = Builders<MongoBlackListItem>.IndexKeys.Ascending(x => x.Key);
            var ttlIndex = Builders<MongoBlackListItem>.IndexKeys.Ascending(x => x.ExpireOn);

            var collection = db.GetCollection<MongoBlackListItem>(nameof(MongoBlackListItem));

            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoBlackListItem>(keyIndex, new CreateIndexOptions() { Unique = true }));
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoBlackListItem>(ttlIndex,
                    new CreateIndexOptions() { ExpireAfter = TimeSpan.FromMinutes(1) }));

            return new MongoBlackList(collection);
        }

        public async Task<bool> Revoke(string key, TimeSpan expireAfter)
        {
            try
            {
                await _blacklist.InsertOneAsync(new MongoBlackListItem(key, DateTime.Now.Add(expireAfter)));
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
                await _blacklist.InsertOneAsync(new MongoBlackListItem(key, expireOn));
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
                await _blacklist.InsertOneAsync(new MongoBlackListItem(key, DateTime.MaxValue));
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
                var delete = await _blacklist.DeleteOneAsync(x => x.Key == key);
                return delete.IsAcknowledged;
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
                await _blacklist.DeleteManyAsync(x => true);
            }
            catch
            {
            }
        }

        public async Task<bool> IsRevoked(string key)
        {
            try
            {
                var item = await _blacklist.Find(x => x.Key == key).SingleAsync();
                return item.ExpireOn > DateTime.Now;
            }
            catch
            {
                return false;
            }
        }
    }
}