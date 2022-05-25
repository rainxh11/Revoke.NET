using Revoke.NET;
using Revoke.NET.Redis;

var store = await RedisBlackListStore.CreateStoreAsync("localhost");

await store.Revoke("Ahmed", DateTimeOffset.MaxValue);

var item = await store.Get<BlackListItem>("Ahmed");

Console.WriteLine(item.Key);

Console.ReadKey();