namespace Revoke.NET.EasyCaching;

using System;
using global::EasyCaching.Core;
using Microsoft.Extensions.DependencyInjection;

public static class RevokeService
{
    public static IServiceCollection AddRevokeEasyCaching(
        this IServiceCollection services,
        IEasyCachingProvider easyCachingProvider,
        TimeSpan? defaultTtl = null)
    {
        return services.AddSingleton<IBlackList, EasyCachingBlackList>(
            _ => new EasyCachingBlackList(easyCachingProvider, defaultTtl));
    }

    public static IServiceCollection AddRevokeEasyCaching(
        this IServiceCollection services,
        Func<IEasyCachingProviderFactory, IEasyCachingProvider> easyCachingConfig,
        TimeSpan? defaultTtl = null)
    {
        return services.AddSingleton<IBlackList, EasyCachingBlackList>(
            provider =>
            {
                IEasyCachingProviderFactory factory = provider.GetService<IEasyCachingProviderFactory>();

                return new EasyCachingBlackList(easyCachingConfig?.Invoke(factory), defaultTtl);
            });
    }

    public static IServiceCollection AddRevokeEasyCaching(
        this IServiceCollection services,
        TimeSpan? defaultTtl = null)
    {
        return services.AddSingleton<IBlackList, EasyCachingBlackList>(
            provider =>
            {
                IEasyCachingProvider easyCachingProvider = provider.GetService<IEasyCachingProvider>();

                return new EasyCachingBlackList(easyCachingProvider, defaultTtl);
            });
    }
}