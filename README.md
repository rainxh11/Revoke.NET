<img src="https://raw.githubusercontent.com/rainxh11/Revoke.NET/master/assets/revoke.net.svg" width="300">

||*NuGet*|
|-|-|
|Revoke.NET|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.svg)](https://www.nuget.org/packages/Revoke.NET/)|
|Revoke.NET.AspNetCore|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.AspNetCore.svg)](https://www.nuget.org/packages/Revoke.NET.AspNetCore/)|
|Revoke.NET.Akavache|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.Akavache.svg)](https://www.nuget.org/packages/Revoke.NET.Akavache/)|
|Revoke.NET.MonkeyCache|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.MonkeyCache.svg)](https://www.nuget.org/packages/Revoke.NET.MonkeyCache/)|
|Revoke.NET.MongoDB|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.MongoDB.svg)](https://www.nuget.org/packages/Revoke.NET.MongoDB/)|
|Revoke.NET.Redis|[![Latest version](https://img.shields.io/nuget/v/Revoke.NET.Redis.svg)](https://www.nuget.org/packages/Revoke.NET.Redis/)

# Revoke.NET
.NET Utility to revoke access based on some given criterias including but not limited to:
- Web Tokens like JWT Bearer token
- HTTP Request Header Paramters, Query, URL, Host, IP, Cookies, Body, FormData, Claims...etc

# Installation
**First**, install the `Revoke.NET` [NuGet package](https://www.nuget.org/packages/Revoke.NET) into your app
```powershell
PM> Install-Package Revoke.NET
```

# How to use
simple create a new BlackList Store of type `IBlackListStore`
```csharp
using Revoke.NET;

var store = MemoryBlackListStore.CreateStore(); 
// Create a blacklist store, core package come with non-persistent in-memory store

var key = "[ID String of something to be blacklisted]";

await store.Revoke(key, TimeSpan.FromHours(24)); // Revoke access to a key for 24 hours

var item = store.Get<SomeType>(key); // Retrieve a blacklisted item, SomeType must implement interface 'IBlackListItem'

await store.Revoke<SomeType>(model); // Revoke an item with custom type

await store.IsRevoked(key); // Check if key is blacklisted

await store.Delete(key); // Delete a key from blacklist
```

# Usage with ASP.NET Core
Install the `Revoke.NET.AspNetCore` [NuGet package](https://www.nuget.org/packages/Revoke.NET.AspNetCore)
```powershell
PM> Install-Package Revoke.NET.AspNetCore
```
### Usage
```csharp
using Revoke.NET;
using Revoke.NET.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRevokeStore(() => /*  provide a BlackList Store */)
    .AddHttpContextRevokeMiddleware(
        context => { /* create custom key selector from HttpContext */ },
        response => { /* create a custom response to be sent when a request is revoked */  }
    ); 
```

### JWT Bearer Token Example
```csharp
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Revoke.NET;
using Revoke.NET.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRevokeInMemoryStore() // Register a Revoke Store
    .AddJWTBearerTokenRevokeMiddleware(); // Register a Revoke Middleware

var app = builder.Build();

app.UseRevoke(); // Use Middleware before calling UseAuthorization()

app.UseAuthorization();
app.UseAuthentication();

app.MapGet("/logout", async ([FromServices] IBlackListStore store, HttpRequest request) =>
{
    var token = AuthenticationHeaderValue.Parse(request.Headers.Authorization).Parameter;

    await store.Revoke(token);

    return true;
});

app.Run();