using Revoke.NET;
using Revoke.NET.Redis;

IBlackList? store = await RedisBlackList.CreateStoreAsync("localhost");

await store.Revoke("Ahmed", DateTime.MaxValue);

bool revoked = await store.IsRevoked("Ahmed");

Console.WriteLine(revoked);

Console.ReadKey();