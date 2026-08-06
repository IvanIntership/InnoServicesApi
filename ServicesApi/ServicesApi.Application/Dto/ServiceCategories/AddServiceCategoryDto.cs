namespace ServicesApi.Application.Dto.ServiceCategories;

public sealed record AddServiceCategoryDto
{
    public string Name { get; init; }
    public TimeSpan Duration { get; init; }
}