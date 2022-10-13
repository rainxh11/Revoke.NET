using Revoke.NET.Redis;

var store = await RedisBlackList.CreateStoreAsync("localhost");

await store.Revoke("Ahmed", DateTime.MaxValue);

var revoked = await store.IsRevoked("Ahmed");

Console.WriteLine(revoked);

Console.ReadKey();