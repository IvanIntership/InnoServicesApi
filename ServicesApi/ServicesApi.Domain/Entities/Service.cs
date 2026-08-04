namespace ServicesApi.Domain.Entities;

public class Service
{
    public Guid Id { get; init; } =  Guid.NewGuid();
    public Guid SpecializationId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}