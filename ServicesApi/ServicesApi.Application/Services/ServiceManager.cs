using AutoMapper;
using ServicesApi.Application.Dto.Services;
using ServicesApi.Application.Dto.Shared;
using ServicesApi.Application.Interfaces;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Exceptions;
using ServicesApi.Domain.Interfaces;

namespace ServicesApi.Application.Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly IProfilesApiClient _profilesApiClient;
    private readonly IMapper _mapper;
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly IDbSession _dbSession;

    public ServiceManager(
        IProfilesApiClient profilesApiClient, 
        IMapper mapper, 
        IServiceRepository serviceRepository, 
        IServiceCategoryRepository serviceCategoryRepository,
        IDbSession dbSession)
    {
        _profilesApiClient = profilesApiClient ?? throw new ArgumentNullException(nameof(profilesApiClient));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
        _serviceCategoryRepository = serviceCategoryRepository ?? throw new ArgumentNullException(nameof(serviceCategoryRepository));
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
    }

    public async Task<ServiceDto> CreateServiceAsync(AddServiceDto addService, CancellationToken ct = default)
    {
        bool alreadyExists = await _serviceRepository.ExistsByNameAsync(addService.Name, ct);
        if (alreadyExists)
        {
            throw new ConflictException($"A service named '{addService.Name}' already exists."); 
        }
        
        bool categoryExists = await _serviceCategoryRepository.ExistsAsync(addService.ServiceCategoryId, ct);
        if (!categoryExists)
        {
            throw new NotFoundException("No such category exists.");
        }
        
        bool specializationExists = await _profilesApiClient.SpecializationExistsAsync(addService.SpecializationId, ct);
        if (!specializationExists)
        {
            throw new NotFoundException("No such specialization exists."); 
        }
        
        var service = _mapper.Map<Service>(addService);

        _dbSession.BeginTransaction();
        try
        {
            await _serviceRepository.AddAsync(service, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
        
        return _mapper.Map<ServiceDto>(service);
    }

    public async Task DeleteServiceAsync(Guid id, CancellationToken ct = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, ct);
        if (service == null)
        {
            throw new NotFoundException("No such service exists.");
        }
        
        _dbSession.BeginTransaction();
        try
        {
            await _serviceRepository.DeleteAsync(id, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
    }

    public async Task<ServiceDto> UpdateServiceAsync(UpdateServiceDto updateService, CancellationToken ct = default)
    {
        bool alreadyExists = await _serviceRepository.ExistsByNameExceptIdAsync(updateService.Id, updateService.Name, ct);
        if (alreadyExists)
        {
            throw new ConflictException($"A service named '{updateService.Name}' already exists."); 
        }
        
        bool categoryExists = await _serviceCategoryRepository.ExistsAsync(updateService.ServiceCategoryId, ct);
        if (!categoryExists)
        {
            throw new NotFoundException("No such category exists.");
        }
        
        bool specializationExists = await _profilesApiClient.SpecializationExistsAsync(updateService.SpecializationId, ct);
        if (!specializationExists)
        {
            throw new NotFoundException("No such specialization exists."); 
        }
        
        var service = _mapper.Map<Service>(updateService);

        _dbSession.BeginTransaction();
        try
        {
            await _serviceRepository.UpdateAsync(service, ct);
            await _dbSession.CommitAsync(ct);
        }
        catch
        {
            _dbSession.Rollback();
            throw;
        }
        
        return _mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> GetServiceByIdAsync(Guid id, CancellationToken ct = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, ct);
        
        if (service == null)
        {
            throw new NotFoundException("No such service exists.");
        }
        var serviceDto = _mapper.Map<ServiceDto>(service);
        serviceDto.Doctors = await _profilesApiClient.GetDoctorsBySpecializationAsync(service.SpecializationId, ct);
        return serviceDto;
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        var services = await _serviceRepository.GetByCategoryId(categoryId, ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesBySpecializationIdAsync(Guid specializationId, CancellationToken ct = default)
    {
        var services = await _serviceRepository.GetBySpecializationId(specializationId, ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesByTermAsync(SearchByTermDto term, CancellationToken ct = default)
    {
        var services = await _serviceRepository.SearchByTerm(term.Term, ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync(CancellationToken ct = default)
    {
        var services = await _serviceRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }
}