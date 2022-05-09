using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Akavache;

namespace Revoke.NET.Akavache
{
    public static class RevokeService
    {
        public static IServiceCollection AddRevokeAkavacheSQLiteStore(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => AkavacheBlackListStore
                    .CreateStoreAsync("RevokeStore", BlobCache.LocalMachine)
                    .GetAwaiter()
                    .GetResult());
        }

        public static IServiceCollection AddRevokeAkavacheInMemoryStore(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => AkavacheBlackListStore
                    .CreateStoreAsync("RevokeStore", BlobCache.InMemory)
                    .GetAwaiter()
                    .GetResult());
        }

        public static IServiceCollection AddRevokeAkavacheStore(this IServiceCollection services,
            Func<IServiceProvider, IBlobCache> configBlobCache)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => AkavacheBlackListStore
                    .CreateStoreAsync("RevokeStore", configBlobCache(provider))
                    .GetAwaiter()
                    .GetResult());
        }
    }
}