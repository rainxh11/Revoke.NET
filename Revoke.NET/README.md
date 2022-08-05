# Revoke.NET
.NET Utility to revoke access based on some given criterias including but not limited to:
- Web Tokens like JWT Bearer token
- HTTP Request Header Paramters, Query, URL, Host, IP, Cookies, Body, FormData, Claims...etc

# Installation
**First**, install the [`Revoke.NET`](https://www.nuget.org/packages/Revoke.NET) into your app
```powershell
Install-Package Revoke.NET
```
or with dotnet cli: 
```powershell
dotnet add package Revoke.NET
```
# How to use
simple create a new BlackList Store of type `IBlackListStore`
```csharp
using Revoke.NET;

var store = MemoryBlackListStore.CreateStore(); 
// Create a blacklist store, core package come with non-persistent in-memory store

var key = "[ID String of something to be blacklisted]";

await store.Revoke(key, TimeSpan.FromHours(24)); // Revoke access to a key for 24 hours

await store.Revoke(key); // Revoke access indefinetly or with the defaulTtl expiration

var revoked = await store.IsRevoked(key); // Check if key is blacklisted

await store.Delete(key); // Delete a key from blacklist
```

# Usage with ASP.NET Core
Install the [`Revoke.NET.AspNetCore`](https://www.nuget.org/packages/Revoke.NET.AspNetCore) into your app
```powershell
Install-Package Revoke.NET.AspNetCore
```
or with dotnet cli: 
```powershell
dotnet add package Revoke.NET.AspNetCore
```
```csharp
using Revoke.NET;

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
```