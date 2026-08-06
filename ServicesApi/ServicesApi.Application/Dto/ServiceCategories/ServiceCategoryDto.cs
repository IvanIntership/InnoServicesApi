namespace ServicesApi.Application.Dto.ServiceCategories;

public sealed record ServiceCategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public TimeSpan Duration { get; init; }
}