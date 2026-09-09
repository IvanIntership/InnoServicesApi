namespace ServicesApi.Application.Dto.Services;

public record GetPagedServicesDto
{
    public string? Term { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}