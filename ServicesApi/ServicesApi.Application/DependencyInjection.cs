using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Application.Mappings;

namespace ServicesApi.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServiceMappingProfile).Assembly));
        
        return services;
    }
}