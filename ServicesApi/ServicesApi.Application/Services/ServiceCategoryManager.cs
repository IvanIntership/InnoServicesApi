using AutoMapper;
using ServicesApi.Application.Dto.ServiceCategories;
using ServicesApi.Application.Dto.Shared;
using ServicesApi.Application.Interfaces;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Exceptions;
using ServicesApi.Domain.Interfaces;

namespace ServicesApi.Application.Services;

public sealed class ServiceCategoryManager : IServiceCategoryManager
{
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly IDbSession _dbSession;
    private readonly IMapper _mapper;

    public ServiceCategoryManager(
        IServiceCategoryRepository serviceCategoryRepository, 
        IDbSession dbSession,
        IMapper mapper)
    {
        _serviceCategoryRepository = serviceCategoryRepository ?? throw new ArgumentNullException(nameof(serviceCategoryRepository));
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ServiceCategoryDto> CreateServiceCategoryAsync(AddServiceCategoryDto addServiceCategory, CancellationToken ct = default)
    {
        var alreadyExists = await _serviceCategoryRepository.ExistsByNameAsync(addServiceCategory.Name, ct);
        if (alreadyExists)
        {
            throw new ConflictException($"A service category named '{addServiceCategory.Name}' already exists.");
        }
        
        var serviceCategory = _mapper.Map<ServiceCategory>(addServiceCategory);

        _dbSession.BeginTransaction();
        try
        {
            await _serviceCategoryRepository.AddAsync(serviceCategory, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
        
        return _mapper.Map<ServiceCategoryDto>(serviceCategory);
    }

    public async Task DeleteServiceCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(id, ct);
        if (serviceCategory == null)
        {
            throw new NotFoundException("No such service category exists.");
        }

        if (await _serviceCategoryRepository.HasAssociatedServicesAsync(serviceCategory.Id, ct))
        {
            throw new ConflictException("The service category with id has associated services.");
        }
        
        _dbSession.BeginTransaction();
        try
        {
            await _serviceCategoryRepository.DeleteAsync(id, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
    }

    public async Task<ServiceCategoryDto> UpdateServiceCategoryAsync(UpdateServiceCategoryDto updateServiceCategory, CancellationToken ct = default)
    {
        var exists = await _serviceCategoryRepository.ExistsAsync(updateServiceCategory.Id, ct);
        if (!exists)
        {
            throw new NotFoundException("This service category doesn't exist.");
        }
        
        var isDuplicate = await _serviceCategoryRepository.ExistsByNameExceptIdAsync(updateServiceCategory.Id, updateServiceCategory.Name, ct);
        if (isDuplicate)
        {
            throw new ConflictException($"The service category named '{updateServiceCategory.Name}' already exists.");
        }
        
        var serviceCategory = _mapper.Map<ServiceCategory>(updateServiceCategory);

        _dbSession.BeginTransaction();
        try
        {
            await _serviceCategoryRepository.UpdateAsync(serviceCategory, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
        
        return _mapper.Map<ServiceCategoryDto>(serviceCategory);
    }

    public async Task<ServiceCategoryDto> GetServiceCategoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(id, ct);
        
        if (serviceCategory == null)
        {
            throw new NotFoundException("No such service category exists.");
        }
        return _mapper.Map<ServiceCategoryDto>(serviceCategory);
    }

    public async Task<IEnumerable<ServiceCategoryDto>> GetServiceCategoriesByTermAsync(SearchByTermDto term, CancellationToken ct = default)
    {
        var serviceCategories = await _serviceCategoryRepository.SearchByTerm(term.Term, ct);
        return _mapper.Map<IEnumerable<ServiceCategoryDto>>(serviceCategories);
    }

    public async Task<IEnumerable<ServiceCategoryDto>> GetAllServiceCategoriesAsync(CancellationToken ct = default)
    {
        var serviceCategories = await _serviceCategoryRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ServiceCategoryDto>>(serviceCategories);
    }

    public async Task<PagedResult<ServiceCategoryDto>> GetServiceCategoriesPagedAsync(
        GetPagedServiceCategoriesDto getPagedServiceCategoriesDto,
        CancellationToken ct = default)
    {
        var searchTerm = getPagedServiceCategoriesDto?.Term?.Trim();
        var pageNumber = getPagedServiceCategoriesDto?.PageNumber ?? 1;
        var pageSize = getPagedServiceCategoriesDto?.PageSize ?? 10;

        var (items, totalCount) = await _serviceCategoryRepository.GetPagedAsync(
            searchTerm, 
            pageNumber, 
            pageSize, 
            ct);

        var dtos = _mapper.Map<IEnumerable<ServiceCategoryDto>>(items);

        return new PagedResult<ServiceCategoryDto>(
            items: dtos, 
            totalCount: totalCount, 
            pageNumber: pageNumber, 
            pageSize: pageSize);
    }
}