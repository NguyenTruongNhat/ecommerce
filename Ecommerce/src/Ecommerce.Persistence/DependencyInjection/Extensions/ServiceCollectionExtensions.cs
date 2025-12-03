using Ecommerce.Domain.Abstractions;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Persistence.DependencyInjection.Options;
using Ecommerce.Persistence.Repositories;
using Ecommerce.Persistence.Repositories.IdentityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ecommerce.Persistence.DependencyInjection.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddSqlConfiguration(this IServiceCollection services)
    {
        services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

            #region ============== SQL-SERVER-STRATEGY-DbContext 1 ==============
            builder
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true)
            .UseLazyLoadingProxies(true)
            .UseSqlServer(
                connectionString: configuration.GetConnectionString("ConnectionStrings"),
                sqlServerOptionsAction: optionsBuilder
                        => optionsBuilder.ExecutionStrategy(
                                dependencies => new SqlServerRetryingExecutionStrategy(
                                    dependencies: dependencies,
                                    maxRetryCount: options.CurrentValue.MaxRetryCount,
                                    maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                                    errorNumbersToAdd: options.CurrentValue.ErrorNumbersToAdd))
                            .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

            #endregion

            #region ============== SQL-SERVER-STRATEGY- UnitOfWork 2 ==============
            //builder
            //.EnableDetailedErrors(true)
            //.EnableSensitiveDataLogging(true)
            //.UseLazyLoadingProxies(true)
            //.UseSqlServer(
            //    connectionString: configuration.GetConnectionString("ConnectionStrings"),
            //    sqlServerOptionsAction: optionsBuilder
            //                => optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));
            #endregion\
        });

    }

    public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptions(this IServiceCollection services, IConfigurationSection section)
    => services
        .AddOptions<SqlServerRetryOptions>()
        .Bind(section)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    public static void AddRepositoryBaseConfiguration(this IServiceCollection services)
    {
        services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
        services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
                .AddTransient<IRoleRepository, RoleRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IVerificationCodeRepository, VerificationCodeRepository>()
                ;

    }
}
