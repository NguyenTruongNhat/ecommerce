using Amazon;
using Amazon.S3;
using Ecommerce.Application.Abstractions;
using Ecommerce.Application.DependencyInjection.Options;
using Ecommerce.Infrastructure.Authentication;
using Ecommerce.Infrastructure.Caching;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Ecommerce.Infrastructure.Hashing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Collections.Specialized.BitVector32;

namespace Ecommerce.Infrastructure.DependencyInjection.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServicesInfrastructure(configuration);
        services.AddRedisInfrastructure(configuration);
        services.AddS3Infrastructure(configuration);

    }

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

    public static void AddS3Infrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region AWS S3 Configuration
        services
            .AddOptions<AwsS3Options>()
            .Bind(configuration.GetSection("AwsS3Options"))
            .ValidateOnStart();
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AwsS3Options>>().Value;
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region)
            };
            var client = new AmazonS3Client(options.AccessKey, options.SecretKey, config);

            // Try a simple ListBuckets call to verify connection and log result
            try
            {
                var buckets = client.ListBucketsAsync().GetAwaiter().GetResult();
                var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("AwsS3Connection");
                logger?.LogInformation("AWS S3 connection successful. Buckets: {BucketCount}", buckets.Buckets.Count);
            }
            catch (Exception ex)
            {
                var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("AwsS3Connection");
                logger?.LogError(ex, "AWS S3 connection failed: {Message}", ex.Message);
            }

            return client;
        });
        #endregion

    }
}
