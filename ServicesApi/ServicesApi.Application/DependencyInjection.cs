using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Application.Interfaces;
using ServicesApi.Application.Mappings;
using ServicesApi.Application.Services;
using ServicesApi.Application.Validation.Services;

namespace ServicesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServiceMappingProfile).Assembly));
        services.AddValidatorsFromAssemblyContaining<ServiceDtoValidator>();

        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<IServiceCategoryManager, ServiceCategoryManager>();

        return services;
    }
}