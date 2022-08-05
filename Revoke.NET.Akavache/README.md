# Revoke.NET Akavache Store
Akavache BlackList Store Extension for [`Revoke.NET`] (https://www.nuget.org/packages/Revoke.NET)

# Installation
Install the `Revoke.NET.Akavache` [NuGet package](https://www.nuget.org/packages/Revoke.NET.Akavache) into your app
```powershell
PM> Install-Package Revoke.NET.Akavache
```

# How to use
```csharp
using Revoke.NET;
using Revoke.NET.Akavache;
using Akavache;

var store = await AkavacheBlackListStore.CreateStoreAsync("RevokeStore", BlobCache.LocalMachine); // Create SQLite3 Persisten BlackList Store
// OR
var store = await AkavacheBlackListStore.CreateStoreAsync("RevokeStore", BlobCache.InMemory); // Create Akavache InMemory Implementation BlackList Store (Non-Persistent)

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
using Revoke.NET.Akavache;
using Revoke.NET.AspNetCore;
using Akavache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRevokeAkavacheInMemoryStore(); // InMemory Store
// OR
builder.Services.AddRevokeAkavacheSQLiteStore(); // SQLite3 Store
// OR
AddRevokeAkavacheStore((provider) => /* Provide Custom Akavache BlobCache */)
```