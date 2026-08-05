using ServicesApi.Domain.Entities;

namespace ServicesApi.Domain.Interfaces;

public interface IServiceCategoryRepository
{
    Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ServiceCategory>> SearchByTerm(string name, CancellationToken сt = default);
    Task<IEnumerable<ServiceCategory>> GetAllAsync(CancellationToken ct = default);
    
    Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default);
    
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}