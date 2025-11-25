using Ecommerce.Application.Abstractions;
using Ecommerce.Infrastructure.Hashing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Infrastructure.DependencyInjection.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    => services.AddTransient<IHashingService, HashingService>();
}
