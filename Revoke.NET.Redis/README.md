# Revoke.NET Redis Store

Redis BlackList Store Extension for [`Revoke.NET`] (https://www.nuget.org/packages/Revoke.NET)

# Installation

Install the `Revoke.NET.Redis` [NuGet package](https://www.nuget.org/packages/Revoke.NET.Redis) into your app

```powershell
PM> Install-Package Revoke.NET.Redis
```

# How to use

```csharp
using Revoke.NET;
using Revoke.NET.Redis;

var store = await RedisBlackListStore.CreateStoreAsync("127.0.0.1:6379");

var key = "[ID String of something to be blacklisted]";

await store.Revoke(key, TimeSpan.FromHours(24)); // Revoke access to a key for 24 hours

await store.IsRevoked(key); // Check if key is blacklisted

await store.Delete(key); // Delete a key from blacklist
```

# Usage with ASP.NET Core

Install the `Revoke.NET.AspNetCore` [NuGet package](https://www.nuget.org/packages/Revoke.NET.AspNetCore)

```powershell
PM> Install-Package Revoke.NET.AspNetCore
```

```csharp
using Revoke.NET;
using Revoke.NET.Redis;
using Revoke.NET.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRevokeRedisStore("server1:6379,server2:6379");
```