using System;
using System.Collections.Generic;
using System.Text;
using EasyCaching.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Revoke.NET.EasyCaching;

namespace Revoke.NET.EasyCaching
{
    public static class RevokeService
    {
        public static IServiceCollection AddRevokeEasyCaching(this IServiceCollection services,
            IEasyCachingProvider easyCachingProvider, TimeSpan? defaultTtl = null)
        {
            return services
                .AddSingleton<IBlackList, EasyCachingBlackList>(provider =>
                    new EasyCachingBlackList(easyCachingProvider, defaultTtl));
        }

        public static IServiceCollection AddRevokeEasyCaching(this IServiceCollection services,
            Func<IEasyCachingProviderFactory, IEasyCachingProvider> easyCachingConfig, TimeSpan? defaultTtl = null)
        {
            return services
                .AddSingleton<IBlackList, EasyCachingBlackList>(provider =>
                {
                    var factory = provider.GetService<IEasyCachingProviderFactory>();
                    return new EasyCachingBlackList(easyCachingConfig?.Invoke(factory), defaultTtl);
                });
        }

        public static IServiceCollection AddRevokeEasyCaching(this IServiceCollection services,
            TimeSpan? defaultTtl = null)
        {
            return services
                .AddSingleton<IBlackList, EasyCachingBlackList>(provider =>
                {
                    var easyCachingProvider = provider.GetService<IEasyCachingProvider>();
                    return new EasyCachingBlackList(easyCachingProvider, defaultTtl);
                });
        }
    }
}