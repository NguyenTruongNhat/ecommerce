using Ecommerce.Application.Abstractions;
using Ecommerce.Application.DependencyInjection.Options;
using Ecommerce.Infrastructure.Authentication;
using Ecommerce.Infrastructure.Caching;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Ecommerce.Infrastructure.Hashing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Collections.Specialized.BitVector32;

namespace Ecommerce.Infrastructure.DependencyInjection.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<GoogleAuthOptions>(configuration.GetSection("GoogleAuth"));
        services
            .AddOptions<GoogleAuthOptions>()
            .Bind(configuration.GetSection("GoogleAuth"))
            .ValidateOnStart();

        services.AddTransient<IHashingService, HashingService>()
               .AddTransient<IJwtTokenService, JwtTokenService>()
               .AddTransient<IAuthenticationService, AuthenticationService>()
               .AddTransient<IGoogleAuthenService, GoogleAuthenService>()
        ;
    }

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
