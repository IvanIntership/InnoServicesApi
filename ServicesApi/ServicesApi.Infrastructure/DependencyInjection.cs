using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Domain.Interfaces;
using ServicesApi.Infrastructure.Http;
using ServicesApi.Infrastructure.Persistence;
using ServicesApi.Infrastructure.Persistence.Repositories;

namespace ServicesApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        
        return services;
    }
}