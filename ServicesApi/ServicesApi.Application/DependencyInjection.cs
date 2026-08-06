using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServicesApi.Application.Validation.Services;

namespace ServicesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ServiceDtoValidator>();
        
        return services;
    }
}