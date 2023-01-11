namespace Revoke.NET.AspNetCore;

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RevokeHttpMiddleware : IMiddleware
{
    private readonly IBlackList _store;
    private readonly Func<HttpContext, string> _selector;

#nullable enable
    private readonly ILogger<RevokeHttpMiddleware>? _logger;
    private readonly Func<HttpResponse, Task<HttpResponse>>? _responseFunc;
#nullable disable

    public RevokeHttpMiddleware(
        IBlackList store,
        ILogger<RevokeHttpMiddleware> logger,
        Func<HttpContext, string> selector)
    {
        this._store = store;
        this._logger = logger;
        this._selector = selector;
    }

    public RevokeHttpMiddleware(
        IBlackList store,
        Func<HttpContext, string> selector)
    {
        this._store = store;
        this._selector = selector;
    }

    public RevokeHttpMiddleware(
        IBlackList store,
        ILogger<RevokeHttpMiddleware> logger,
        Func<HttpContext, string> selector,
        Func<HttpResponse, Task<HttpResponse>> responseFunc)
    {
        this._store = store;
        this._logger = logger;
        this._selector = selector;
        this._responseFunc = responseFunc;
    }

    public RevokeHttpMiddleware(
        IBlackList store,
        Func<HttpContext, string> selector,
        Func<HttpResponse, Task<HttpResponse>> responseFunc)
    {
        this._store = store;
        this._selector = selector;
        this._responseFunc = responseFunc;
    }

    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        try
        {
            string revokeKey = this._selector(context);
            if (revokeKey != null)
            {
                if (await this._store.IsRevoked(revokeKey))
                {
                    if (this._responseFunc != null)
                    {
                        await this._responseFunc(context.Response);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }

                    this._logger?.LogInformation("[Revoke.NET] Revoked Access to key: \'{RevokeKey}\'", revokeKey);
                }
                else
                {
                    await next(context);
                }
            }
            else
            {
                await next(context);
            }
        }
        catch (Exception ex)
        {
            this._logger?.LogError("{ErrorMessage}", ex.Message);
            await next(context);
        }
    }
}