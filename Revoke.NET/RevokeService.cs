using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Revoke.NET
{
    public static class RevokeService
    {
        /// <summary>
        /// Register default InMemory BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultTtl">Default Blacklist Item Expiration Duration, null mean no expiration</param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeInMemoryStore(this IServiceCollection services,
            TimeSpan? defaultTtl = null)
        {
            return services
                .AddSingleton<IBlackListStore>(MemoryBlackListStore.CreateStore(defaultTtl));
        }

        /// <summary>
        /// Register BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeStore(this IServiceCollection services,
            Func<IBlackListStore> configure)
        {
            return services
                .AddSingleton<IBlackListStore>(configure());
        }

        /// <summary>
        /// Register BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeStore(this IServiceCollection services,
            Func<IServiceProvider, IBlackListStore> configure)
        {
            return services
                .AddSingleton<IBlackListStore>(configure);
        }

        /// <summary>
        /// Register a Revoke Http Middleware with a Custom Value Selector from an HttpContext 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="selector">Selector function that returns a key to determine if request will be denied, null/empty values to skip</param>
        /// <returns></returns>
        public static IServiceCollection AddHttpContextRevokeMiddleware(this IServiceCollection services,
            Func<HttpContext, string> selector)
        {
            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, selector);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with a Custom Value Selector from an HttpContext 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="selector">Selector function that returns a key to determine if request will be denied, null/empty values to skip</param>
        /// <param name="responseFunc">custom response function</param>
        /// <returns></returns>
        public static IServiceCollection AddHttpContextRevokeMiddleware(this IServiceCollection services,
            Func<HttpContext, string> selector, Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, selector, responseFunc);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with default JWT Bearer Token Selector 'Authorization : Bearer %TOKEN%'
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTBearerTokenRevokeMiddleware(this IServiceCollection services)
        {
            var bearerTokenSelector = new Func<HttpContext, string>(context =>
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var jwtToken = AuthenticationHeaderValue.Parse(authHeader).Parameter;
                    if (jwtToken != null) return "JWT_" + jwtToken;
                }

                return null;
            });

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, bearerTokenSelector);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with default JWT Bearer Token Selector 'Authorization : Bearer %TOKEN%'
        /// </summary>
        /// <param name="services"></param>
        /// <param name="responseFunc">custom response function</param>
        /// <returns></returns>
        public static IServiceCollection AddJWTBearerTokenRevokeMiddleware(this IServiceCollection services,
            Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            var bearerTokenSelector = new Func<HttpContext, string>(context =>
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var jwtToken = AuthenticationHeaderValue.Parse(authHeader).Parameter;
                    if (jwtToken != null) return "JWT_" + jwtToken;
                }

                return null;
            });

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, bearerTokenSelector, responseFunc);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with IP/Host selector
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIPRevokeMiddleware(this IServiceCollection services)
        {
            var ipSelector = new Func<HttpContext, string>(context => "IP_" + context.Request.Host.Host);

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, ipSelector);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with IP/Host selector
        /// </summary>
        /// <param name="services"></param>
        /// <param name="responseFunc">custom response function</param>
        /// <returns></returns>
        public static IServiceCollection AddIPRevokeMiddleware(this IServiceCollection services,
            Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            var ipSelector = new Func<HttpContext, string>(context => "IP_" + context.Request.Host.Host);

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, ipSelector, responseFunc);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with User ID selector
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserIdRevokeMiddleware(this IServiceCollection services)
        {
            var ipSelector = new Func<HttpContext, string>(context => "USERID_" + context.Request.Host.Host);

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, ipSelector);
                });
        }

        /// <summary>
        /// Register a Revoke Http Middleware with User ID selector
        /// </summary>
        /// <param name="services"></param>
        /// <param name="responseFunc">custom response function</param>
        /// <returns></returns>
        public static IServiceCollection AddUserIdRevokeMiddleware(this IServiceCollection services,
            Func<HttpResponse, Task<HttpResponse>> responseFunc)
        {
            var ipSelector = new Func<HttpContext, string>(context => "USERID_" + context.Request.Host.Host);

            return services
                .AddSingleton<RevokeHttpMiddleware>(provider =>
                {
                    var store = provider.GetService<IBlackListStore>();
                    var logger = provider.GetService<ILogger<RevokeHttpMiddleware>>();
                    return new RevokeHttpMiddleware(store, logger, ipSelector, responseFunc);
                });
        }

        /// <summary>
        /// Use Revoke Http Middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRevoke(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<RevokeHttpMiddleware>();
        }
    }
}