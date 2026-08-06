using FluentValidation;
using ServicesApi.Application.Dto.Shared;

namespace ServicesApi.Application.Validation.Shared;

public sealed class SearchByTermDtoValidator : AbstractValidator<SearchByTermDto>
{
    public SearchByTermDtoValidator()
    {
        RuleFor(x => x.Term)
            .MaximumLength(100).WithMessage("The search query is too long.")
            .MinimumLength(1).WithMessage("You need to enter at least 1 character to search.");
    }
}