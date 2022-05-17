using Microsoft.Extensions.DependencyInjection;

namespace Revoke.NET.Redis
{
    public static class RevokeService
    {
        public static IServiceCollection AddRevokeRedisStore(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => RedisBlackListStore.CreateStoreAsync("127.0.0.1:6379")
                    .GetAwaiter()
                    .GetResult());
        }

        public static IServiceCollection AddRevokeRedisStore(this IServiceCollection services, string connectionString)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => RedisBlackListStore.CreateStoreAsync(connectionString)
                    .GetAwaiter()
                    .GetResult());
        }
    }
}