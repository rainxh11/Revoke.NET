namespace Revoke.NET.MongoDB;

using global::MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

public static class RevokeService
{
    public static IServiceCollection AddRevokeMongoStore(this IServiceCollection services)
    {
        return services.AddSingleton(
            _ => MongoBlackList.CreateStoreAsync("RevokeStore", MongoClientSettings.FromConnectionString("mongodb://127.0.0.1:27017/RevokeStore"))
                .GetAwaiter()
                .GetResult());
    }

    public static IServiceCollection AddRevokeMongoStore(this IServiceCollection services, string dbName, MongoClientSettings settings)
    {
        return services.AddSingleton(
            _ => MongoBlackList.CreateStoreAsync(dbName, settings)
                .GetAwaiter()
                .GetResult());
    }
}