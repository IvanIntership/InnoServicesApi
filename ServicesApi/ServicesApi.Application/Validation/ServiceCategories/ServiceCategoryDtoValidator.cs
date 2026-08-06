using FluentValidation;
using ServicesApi.Application.Dto.ServiceCategories;

namespace ServicesApi.Application.Validation.ServiceCategories;

public sealed class ServiceCategoryDtoValidator : AbstractValidator<ServiceCategoryDto>
{
    public ServiceCategoryDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id must not be empty.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
        
        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Service category duration is required.")
            .InclusiveBetween(TimeSpan.FromMinutes(5), TimeSpan.FromHours(8)).WithMessage("Duration must be between 5 minutes and 8 hours.");
    }
}