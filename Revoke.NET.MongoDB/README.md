# Revoke.NET MongoDB Store
MongoDB BlackList Store Extension for [`Revoke.NET`] (https://www.nuget.org/packages/Revoke.NET)

# Installation
Install the `Revoke.NET.MongoDB` [NuGet package](https://www.nuget.org/packages/Revoke.NET.MongoDB) into your app
```powershell
PM> Install-Package Revoke.NET.MongoDB
```

# How to use
```csharp
using MongoDB.Driver;
using Revoke.NET;
using Revoke.NET.MongoDB;

var store = await MongoBlackListStore.CreateStoreAsync("RevokeStore",
    MongoClientSettings.FromConnectionString("mongodb://127.0.0.1:27017/RevokeStore"));

var key = "[ID String of something to be blacklisted]";

await store.Revoke(key, TimeSpan.FromHours(24)); // Revoke access to a key for 24 hours

await store.Revoke<SomeType>(model); // Revoke an item with custom type

var item = store.Get<SomeType>(key); // Retrieve a blacklisted item, SomeType must implement interface 'IBlackListItem'

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
using Revoke.NET.MongoDB;
using Revoke.NET.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRevokeMongoStore();
```