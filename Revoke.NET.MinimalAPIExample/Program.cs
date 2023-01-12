using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Revoke.NET;
using Revoke.NET.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRevokeInMemoryStore() // Register a Revoke Store
    .AddJWTBearerTokenRevokeMiddleware(); // Register a Revoke Middleware

WebApplication app = builder.Build();

app.UseRevoke(); // Use Middleware before calling UseAuthorization()

app.UseAuthorization();
app.UseAuthentication();

app.MapGet(
    "/logout",
    async (
        [FromServices] IBlackList store,
        HttpRequest request) =>
    {
        string? token = AuthenticationHeaderValue.Parse(request.Headers.Authorization)
            .Parameter;

        await store.Revoke(token);

        return true;
    });

app.Run();