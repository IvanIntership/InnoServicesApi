namespace ServicesApi.Application.Dto.Services;

public sealed record UpdateServiceDto
{
    public Guid Id { get; init; }
    public Guid SpecializationId { get; init; }
    public Guid ServiceCategoryId { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}