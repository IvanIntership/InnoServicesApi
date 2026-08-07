using ServicesApi.Application.Dto.ServiceCategories;
using ServicesApi.Application.Dto.Shared;

namespace ServicesApi.Application.Interfaces;

public interface IServiceCategoryManager
{
    Task<ServiceCategoryDto> CreateServiceCategoryAsync(AddServiceCategoryDto addServiceCategory, CancellationToken ct = default);
    
    Task DeleteServiceCategoryAsync(Guid id, CancellationToken ct = default);
    
    Task<ServiceCategoryDto> UpdateServiceCategoryAsync(UpdateServiceCategoryDto updateServiceCategory, CancellationToken ct = default);
    
    Task<ServiceCategoryDto> GetServiceCategoryByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ServiceCategoryDto>> GetServiceCategoriesByTermAsync(SearchByTermDto term, CancellationToken ct = default);
    Task<IEnumerable<ServiceCategoryDto>> GetAllServiceCategoriesAsync(CancellationToken ct = default);
}