using Microsoft.Extensions.Caching.Memory;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class CachedServiceRepository : IServiceRepository
{
    private readonly IServiceRepository _inner;
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public CachedServiceRepository(IServiceRepository inner, IMemoryCache memoryCache)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = $"service-{id}";

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var service = await _inner.GetByIdAsync(id, ct);

            if (service is null)
            {
                entry.SetAbsoluteExpiration(TimeSpan.Zero);
                return null;
            }

            entry.SetAbsoluteExpiration(CacheDuration);
            return service;
        });
    }

    public async Task<IEnumerable<Service>> GetByCategoryId(Guid categoryId, CancellationToken ct = default)
    {
        string cacheKey = $"services-category-{categoryId}";

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _inner.GetByCategoryId(categoryId, ct);
        }) ?? Enumerable.Empty<Service>();
    }

    public async Task<IEnumerable<Service>> GetBySpecializationId(Guid specializationId, CancellationToken ct = default)
    {
        string cacheKey = $"services-specialization-{specializationId}";

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _inner.GetBySpecializationId(specializationId, ct);
        }) ?? Enumerable.Empty<Service>();
    }

    public Task<IEnumerable<Service>> SearchByTerm(string term, CancellationToken ct = default) => _inner.SearchByTerm(term, ct);

    public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken ct = default)
    {
        string cacheKey = "services-all";

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _inner.GetAllAsync(ct);
        }) ?? Enumerable.Empty<Service>();
    }

    public async Task<bool> UpdateAsync(Service service, CancellationToken ct = default)
    {
        var updated = await _inner.UpdateAsync(service, ct);
        
        if(updated)
        {
            _memoryCache.Remove($"service-{service.Id}");
            _memoryCache.Remove($"services-category-{service.ServiceCategoryId}");
            _memoryCache.Remove($"services-specialization-{service.SpecializationId}");
            _memoryCache.Remove("services-all");
        }
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var serviceToDelete = await _inner.GetByIdAsync(id, ct);
        
        var deleted = await _inner.DeleteAsync(id, ct);
        if(deleted)
        {
            _memoryCache.Remove($"service-{id}");
            _memoryCache.Remove("services-all");

            if (serviceToDelete is not null)
            {
                _memoryCache.Remove($"services-category-{serviceToDelete.ServiceCategoryId}");
                _memoryCache.Remove($"services-specialization-{serviceToDelete.SpecializationId}");
            }
        }
        return deleted;
    }

    public async Task<bool> AddAsync(Service service, CancellationToken ct = default)
    {
        var added = await _inner.AddAsync(service, ct);
        
        if(added)
        {
            _memoryCache.Remove($"services-category-{service.ServiceCategoryId}");
            _memoryCache.Remove($"services-specialization-{service.SpecializationId}");
            _memoryCache.Remove("services-all");
        }
        return added;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _inner.ExistsAsync(id, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) => _inner.ExistsByNameAsync(name, ct);
    public Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default) => _inner.ExistsByNameExceptIdAsync(id, name, ct);
}