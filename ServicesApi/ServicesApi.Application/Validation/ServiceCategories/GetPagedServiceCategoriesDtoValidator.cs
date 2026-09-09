using FluentValidation;
using ServicesApi.Application.Dto.ServiceCategories;

namespace ServicesApi.Application.Validation.ServiceCategories;

public sealed class GetPagedServiceCategoriesDtoValidator : AbstractValidator<GetPagedServiceCategoriesDto>
{
    public GetPagedServiceCategoriesDtoValidator()
    {
        RuleFor(x => x.Term)
            .MaximumLength(100).WithMessage("The search query is too long.");
        
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}