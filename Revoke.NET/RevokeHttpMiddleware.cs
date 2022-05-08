using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Revoke.NET
{
    public class RevokeHttpMiddleware : IMiddleware
    {
        private readonly IBlackListStore store;
        private readonly Func<HttpContext, string> selector;

#nullable enable
        private readonly ILogger<RevokeHttpMiddleware>? logger;
        private Func<HttpResponse, Task<HttpResponse>>? responseFunc;
#nullable disable

        public RevokeHttpMiddleware(IBlackListStore store, ILogger<RevokeHttpMiddleware> logger,
            Func<HttpContext, string> selector)
        {
            this.store = store;
            this.logger = logger;
            this.selector = selector;
        }

        public RevokeHttpMiddleware(IBlackListStore store, Func<HttpContext, string> selector)
        {
            this.store = store;
            this.selector = selector;
        }

        public RevokeHttpMiddleware(IBlackListStore store, ILogger<RevokeHttpMiddleware> logger,
            Func<HttpContext, string> selector, Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            this.store = store;
            this.logger = logger;
            this.selector = selector;
            this.responseFunc = responseFunc;
        }

        public RevokeHttpMiddleware(IBlackListStore store, Func<HttpContext, string> selector,
            Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            this.store = store;
            this.selector = selector;
            this.responseFunc = responseFunc;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var revokeKey = selector(context);
                if (revokeKey != null)
                {
                    if (await store.IsRevoked(revokeKey))
                    {
                        var item = await store.Get<IBlackListItem>(revokeKey);

                        logger.LogInformation(
                            $"[Revoke.NET] Revoked Access to key: '{revokeKey}', Blacklisting will be lifted on {item.ExpireOn}");
                        if (responseFunc != null)
                        {
                            await responseFunc(context.Response);
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
            }
            finally
            {
                await next(context);
            }
        }
    }
}