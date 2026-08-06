namespace ServicesApi.Application.Dto.Shared;

public sealed record SearchByTermDto
{
    public string Term { get; init; }
}