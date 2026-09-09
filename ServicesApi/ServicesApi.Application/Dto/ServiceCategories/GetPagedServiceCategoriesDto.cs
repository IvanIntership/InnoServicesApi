namespace ServicesApi.Application.Dto.ServiceCategories;

public sealed record GetPagedServiceCategoriesDto
{
    public string? Term { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}