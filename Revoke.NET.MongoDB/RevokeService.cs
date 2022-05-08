using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Revoke.NET.MongoDB
{
    public static class RevokeService
    {
        public static IServiceCollection AddRevokeMongoStore(this IServiceCollection services)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => MongoBlackListStore.CreateStoreAsync(
                        "RevokeStore",
                        MongoClientSettings.FromConnectionString("mongodb://127.0.0.1:27017/RevokeStore"))
                    .GetAwaiter()
                    .GetResult());
        }

        public static IServiceCollection AddRevokeMongoStore(this IServiceCollection services, string dbName,
            MongoClientSettings settings)
        {
            return services
                .AddSingleton<IBlackListStore>(provider => MongoBlackListStore.CreateStoreAsync(
                        dbName,
                        settings)
                    .GetAwaiter()
                    .GetResult());
        }
    }
}