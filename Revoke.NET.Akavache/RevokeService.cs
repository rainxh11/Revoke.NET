namespace Revoke.NET.Akavache;

using System;
using global::Akavache;
using Microsoft.Extensions.DependencyInjection;

public static class RevokeService
{
    public static IServiceCollection AddRevokeAkavacheSQLiteStore(this IServiceCollection services)
    {
        return services.AddSingleton(
            provider => AkavacheBlackList.CreateStoreAsync("RevokeStore", BlobCache.LocalMachine)
                .GetAwaiter()
                .GetResult());
    }

    public static IServiceCollection AddRevokeAkavacheInMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton(
            provider => AkavacheBlackList.CreateStoreAsync("RevokeStore", BlobCache.InMemory)
                .GetAwaiter()
                .GetResult());
    }

    public static IServiceCollection AddRevokeAkavacheStore(this IServiceCollection services, Func<IServiceProvider, IBlobCache> configBlobCache)
    {
        return services.AddSingleton(
            provider => AkavacheBlackList.CreateStoreAsync("RevokeStore", configBlobCache(provider))
                .GetAwaiter()
                .GetResult());
    }
}