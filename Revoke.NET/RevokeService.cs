using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Revoke.NET
{
    public static class RevokeService
    {
        public static IServiceCollection AddRevokeMemoryCacheStore(this IServiceCollection services,
            TimeSpan? defaultTtl = null)
        {
            services.TryAddSingleton<IBlackList>(provider =>
                new MemoryCacheBlackList(provider.GetService<IMemoryCache>(), defaultTtl));
            return services;
        }

        /// <summary>
        /// Register default InMemory BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultTtl">Default Blacklist Item Expiration Duration, null mean no expiration</param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeInMemoryStore(this IServiceCollection services,
            TimeSpan? defaultTtl = null)
        {
            services
                .TryAddSingleton<IBlackList>(MemoryBlackList.CreateStore(defaultTtl));
            return services;
        }

        /// <summary>
        /// Register BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeStore(this IServiceCollection services,
            Func<IBlackList> configure)
        {
            services
                .TryAddSingleton<IBlackList>(configure());
            return services;
        }

        /// <summary>
        /// Register BlackList Store Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRevokeStore(this IServiceCollection services,
            Func<IServiceProvider, IBlackList> configure)
        {
            services
                .TryAddSingleton<IBlackList>(configure);
            return services;
        }
    }
}