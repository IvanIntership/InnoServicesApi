using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Domain.Interfaces;
using ServicesApi.Infrastructure.Persistence;
using ServicesApi.Infrastructure.Persistence.Repositories;

namespace ServicesApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "ServicesApi_";
        });
        
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.Decorate<IServiceCategoryRepository, CachedServiceCategoryRepository>();
        services.Decorate<IServiceRepository, CachedServiceRepository>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
}