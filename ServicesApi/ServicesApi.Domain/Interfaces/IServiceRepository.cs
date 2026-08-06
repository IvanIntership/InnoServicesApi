using ServicesApi.Domain.Entities;

namespace ServicesApi.Domain.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Service>> GetByCategoryId(Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<Service>> GetBySpecializationId(Guid specializationId, CancellationToken ct = default);
    Task<IEnumerable<Service>> SearchByTerm(string term, CancellationToken ct = default);
    Task<IEnumerable<Service>> GetAllAsync(CancellationToken ct = default);
    
    Task UpdateAsync(Service service, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Service service, CancellationToken ct = default);
    
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}