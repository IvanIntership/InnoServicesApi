using ServicesApi.Application.Dto.Services;
using ServicesApi.Application.Dto.Shared;

namespace ServicesApi.Application.Interfaces;

public interface IServiceManager
{
    Task<ServiceDto> CreateServiceAsync(AddServiceDto addService, CancellationToken ct = default);
    
    Task DeleteServiceAsync(Guid id, CancellationToken ct = default);
    
    Task<ServiceDto> UpdateServiceAsync(UpdateServiceDto updateService, CancellationToken ct = default);
    
    Task<ServiceDto> GetServiceByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ServiceDto>> GetServicesByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<ServiceDto>> GetServicesBySpecializationIdAsync(Guid specializationId, CancellationToken ct = default);
    Task<IEnumerable<ServiceDto>> GetServicesByTermAsync(SearchByTermDto term, CancellationToken ct = default);
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync(CancellationToken ct = default);
}