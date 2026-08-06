using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Application.Dto.Services;

namespace ServicesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ServiceDto>();
        
        return services;
    }
}