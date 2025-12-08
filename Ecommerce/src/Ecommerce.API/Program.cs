using Ecommerce.API.DependencyInjection.Extensions;
using Ecommerce.API.Middleware;
using Ecommerce.Application.DependencyInjection.Extensions;
using Ecommerce.Application.DependencyInjection.Options;
using Ecommerce.Infrastructure.DependencyInjection.Extensions;
using Ecommerce.Persistence.DependencyInjection.Extensions;
using Ecommerce.Persistence.DependencyInjection.Options;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

// Add services to the container.
builder.Services.AddConfigureMediatR();

builder
    .Services
    .AddControllers()
    .AddApplicationPart(Ecommerce.Presentation.AssemblyReference.Assembly);

// Configure Options and SQL
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlConfiguration();

builder.Services.ConfigureOtpOptions(builder.Configuration.GetSection(nameof(OtpOptions)));

builder.Services.AddRepositoryBaseConfiguration();

builder.Services.AddConfigureAutoMapper();

builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwagger();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Configure Authentication & Authorization
builder.Services.AddJwtAuthenticationAPI(builder.Configuration);

// Configure Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);

// Configure IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.ConfigureSwagger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
