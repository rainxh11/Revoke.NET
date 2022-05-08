# Revoke.NET ASP.NET Core Extension
MongoDB BlackList Store Extension for [`Revoke.NET`] (https://www.nuget.org/packages/Revoke.NET)

# Installation
Install the `Revoke.NET.AspNetCore` [NuGet package](https://www.nuget.org/packages/Revoke.NET.AspNetCore) into your app
```powershell
PM> Install-Package Revoke.NET.AspNetCore
```

# Usage with ASP.NET Core
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
```