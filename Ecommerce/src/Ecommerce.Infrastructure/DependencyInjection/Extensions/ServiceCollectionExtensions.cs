using Ecommerce.Application.Abstractions;
using Ecommerce.Application.DependencyInjection.Options;
using Ecommerce.Infrastructure.Caching;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Ecommerce.Infrastructure.Hashing;
using Ecommerce.Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

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

    public static void AddMailInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var resendOption = new ResendOption();
        configuration.GetSection(nameof(ResendOption)).Bind(resendOption);

        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = resendOption.ApiToken;
        });
        services.AddTransient<IResend, ResendClient>();

        services.AddTransient<IMailService, MailService>();

    }

}
