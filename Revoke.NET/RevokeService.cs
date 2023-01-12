namespace Revoke.NET;

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
///     The revoke service class
/// </summary>
public static class RevokeService
{
    /// <summary>
    ///     Register default InMemory BlackList Store Service using <seealso cref="MemoryCacheBlackList" />
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="defaultTtl">The default ttl</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddRevokeMemoryCacheStore(
        this IServiceCollection services,
        TimeSpan? defaultTtl = null)
    {
        services.TryAddSingleton<IBlackList>(
            provider => new MemoryCacheBlackList(provider.GetService<IMemoryCache>(), defaultTtl));

        return services;
    }

    /// <summary>
    ///     Register default InMemory BlackList Store Service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="defaultTtl">Default Blacklist Item Expiration Duration, null mean no expiration</param>
    /// <returns></returns>
    public static IServiceCollection AddRevokeInMemoryStore(
        this IServiceCollection services,
        TimeSpan? defaultTtl = null)
    {
        services.TryAddSingleton<IBlackList>(MemoryBlackList.CreateStore(defaultTtl));

        return services;
    }

    /// <summary>
    ///     Register BlackList Store Service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddRevokeStore(
        this IServiceCollection services,
        Func<IBlackList> configure)
    {
        services.TryAddSingleton(configure());

        return services;
    }

    /// <summary>
    ///     Register BlackList Store Service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddRevokeStore(
        this IServiceCollection services,
        Func<IServiceProvider, IBlackList> configure)
    {
        services.TryAddSingleton(configure);

        return services;
    }
}