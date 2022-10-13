namespace Revoke.NET.Redis;

using Microsoft.Extensions.DependencyInjection;

public static class RevokeService
{
    public static IServiceCollection AddRevokeRedisStore(this IServiceCollection services)
    {
        return services.AddSingleton(
            provider => RedisBlackList.CreateStoreAsync("127.0.0.1:6379")
                .GetAwaiter()
                .GetResult());
    }

    public static IServiceCollection AddRevokeRedisStore(this IServiceCollection services, string connectionString)
    {
        return services.AddSingleton(
            provider => RedisBlackList.CreateStoreAsync(connectionString)
                .GetAwaiter()
                .GetResult());
    }
}