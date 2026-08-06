using FluentValidation;
using ServicesApi.Application.Dto.Services;

namespace ServicesApi.Application.Validation.Services;

public sealed class ServiceDtoValidator : AbstractValidator<ServiceDto>
{
    public ServiceDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id must not be empty.");
        
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("Specialization Id must not be empty.");
        
        RuleFor(x => x.ServiceCategoryId).NotEmpty().WithMessage("Service category Id must not be empty.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required.")
            .MaximumLength(50).WithMessage("Service name cannot exceed 50 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo((decimal)0.01)
            .WithMessage("Price must be at least 0.01.")
            .LessThanOrEqualTo(100000)
            .WithMessage("Price cannot exceed 100.000.");
    }
}