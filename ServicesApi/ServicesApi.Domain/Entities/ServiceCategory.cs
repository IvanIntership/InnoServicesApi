namespace ServicesApi.Domain.Entities;

public sealed class ServiceCategory
{
    public Guid Id { get; init; } =  Guid.NewGuid();
    public string Name { get; set; }
    public TimeSpan Duration { get; set; }
}