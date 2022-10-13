namespace Revoke.NET.MongoDB;

using System;
using System.Threading.Tasks;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using global::MongoDB.Driver;

public class MongoBlackListItem
{
    public MongoBlackListItem(string key, DateTime expireOn)
    {
        this.Key = key;
        this.ExpireOn = expireOn;
    }

    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

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

    public async Task<bool> Revoke(string key, TimeSpan expireAfter)
    {
        try
        {
            await this._blacklist.InsertOneAsync(new MongoBlackListItem(key, DateTime.Now.Add(expireAfter)));

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
            await this._blacklist.InsertOneAsync(new MongoBlackListItem(key, expireOn));

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
            await this._blacklist.InsertOneAsync(new MongoBlackListItem(key, DateTime.MaxValue));

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
            var delete = await this._blacklist.DeleteOneAsync(x => x.Key == key);

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
            await this._blacklist.DeleteManyAsync(x => true);
        }
        catch
        {
            // ignored
        }
    }

    public async Task<bool> IsRevoked(string key)
    {
        try
        {
            var item = await this._blacklist.Find(x => x.Key == key)
                .SingleAsync();

            return item.ExpireOn > DateTime.Now;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<IBlackList> CreateStoreAsync(string dbName, MongoClientSettings clientSettings)
    {
        var client = new MongoClient(clientSettings);

        var db = client.GetDatabase(dbName);

        var keyIndex = Builders<MongoBlackListItem>.IndexKeys.Ascending(x => x.Key);
        var ttlIndex = Builders<MongoBlackListItem>.IndexKeys.Ascending(x => x.ExpireOn);

        var collection = db.GetCollection<MongoBlackListItem>(nameof(MongoBlackListItem));

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoBlackListItem>(
                keyIndex,
                new CreateIndexOptions
                {
                    Unique = true
                }));
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoBlackListItem>(
                ttlIndex,
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.FromMinutes(1)
                }));

        return new MongoBlackList(collection);
    }
}