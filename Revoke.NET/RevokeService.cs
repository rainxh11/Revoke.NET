using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
    }
}