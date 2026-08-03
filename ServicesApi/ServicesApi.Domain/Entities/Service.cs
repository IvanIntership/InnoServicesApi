namespace ServicesApi.Domain.Entities;

public class Service
{
    public Guid Id { get; init; } =  Guid.NewGuid();
    public Guid ServiceId { get; set; }
    public Guid ServiceCategory { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}