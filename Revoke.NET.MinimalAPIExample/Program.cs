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

app.MapGet("/logout", async ([FromServices] IBlackList store, HttpRequest request) =>
{
    var token = AuthenticationHeaderValue.Parse(request.Headers.Authorization).Parameter;

    await store.Revoke(token);

    return true;
});

app.Run();