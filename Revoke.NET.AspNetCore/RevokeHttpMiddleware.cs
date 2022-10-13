namespace Revoke.NET.AspNetCore;

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RevokeHttpMiddleware : IMiddleware
{
    private readonly IBlackList store;
    private readonly Func<HttpContext, string> selector;

#nullable enable
    private readonly ILogger<RevokeHttpMiddleware>? logger;
    private readonly Func<HttpResponse, Task<HttpResponse>>? responseFunc;
#nullable disable

    public RevokeHttpMiddleware(IBlackList store, ILogger<RevokeHttpMiddleware> logger, Func<HttpContext, string> selector)
    {
        this.store = store;
        this.logger = logger;
        this.selector = selector;
    }

    public RevokeHttpMiddleware(IBlackList store, Func<HttpContext, string> selector)
    {
        this.store = store;
        this.selector = selector;
    }

    public RevokeHttpMiddleware(
        IBlackList store, ILogger<RevokeHttpMiddleware> logger, Func<HttpContext, string> selector,
        Func<HttpResponse, Task<HttpResponse>> responseFunc)
    {
        this.store = store;
        this.logger = logger;
        this.selector = selector;
        this.responseFunc = responseFunc;
    }

    public RevokeHttpMiddleware(IBlackList store, Func<HttpContext, string> selector, Func<HttpResponse, Task<HttpResponse>> responseFunc)
    {
        this.store = store;
        this.selector = selector;
        this.responseFunc = responseFunc;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var revokeKey = this.selector(context);
            if (revokeKey != null)
            {
                if (await this.store.IsRevoked(revokeKey))
                {
                    if (this.responseFunc != null)
                    {
                        await this.responseFunc(context.Response);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }

                    this.logger.LogInformation($"[Revoke.NET] Revoked Access to key: '{revokeKey}'");
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
            this.logger?.LogError(ex.Message);
            await next(context);
        }
    }
}