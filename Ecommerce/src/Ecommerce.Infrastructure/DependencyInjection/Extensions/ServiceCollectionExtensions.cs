using Ecommerce.Application.Abstractions;
using Ecommerce.Infrastructure.Caching;
using Ecommerce.Infrastructure.Hashing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Infrastructure.DependencyInjection.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    => services.AddTransient<IHashingService, HashingService>();

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICacheService, CacheService>()
                .AddStackExchangeRedisCache(redisOptions =>
                {
                    var connectionString = configuration.GetConnectionString("Redis");
                    redisOptions.Configuration = connectionString;
                });
    }
}
