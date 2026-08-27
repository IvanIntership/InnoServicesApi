using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ServicesApi.Application.Interfaces;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;
using ServicesApi.Infrastructure.Persistence.Constants;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class CachedServiceRepository : IServiceRepository
{
    private readonly IServiceRepository _inner;
    private readonly IDistributedCache _distributedCache;
    private readonly IDbSession _dbSession;
    
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public CachedServiceRepository(IServiceRepository inner, IDistributedCache distributedCache, IDbSession dbSession)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _dbSession = dbSession;
    }

    private async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, CancellationToken ct)
    {
        var cachedJson = await _distributedCache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<T>(cachedJson);
        }

        var result = await factory();
        if (result is not null)
        {
            var json = JsonSerializer.Serialize(result);
            await _distributedCache.SetStringAsync(key, json, CacheOptions, ct);
        }

        return result;
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.ServiceById(id);
        return await GetOrCreateAsync(cacheKey, () => _inner.GetByIdAsync(id, ct), ct);
    }

    public async Task<IEnumerable<Service>> GetByCategoryId(Guid categoryId, CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.ServicesByCategoryId(categoryId);
        var services = await GetOrCreateAsync<IEnumerable<Service>>(
            cacheKey, 
            async () => await _inner.GetByCategoryId(categoryId, ct), 
            ct);
        
        return services ?? Enumerable.Empty<Service>();
    }

    public async Task<IEnumerable<Service>> GetBySpecializationId(Guid specializationId, CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.ServicesBySpecializationId(specializationId);
        var services = await GetOrCreateAsync<IEnumerable<Service>>(
            cacheKey, 
            async () => await _inner.GetBySpecializationId(specializationId, ct), 
            ct);
            
        return services ?? Enumerable.Empty<Service>();
    }

    public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.ServicesAll;
        var services = await GetOrCreateAsync<IEnumerable<Service>>(
            cacheKey, 
            async () => await _inner.GetAllAsync(ct), 
            ct);
            
        return services ?? Enumerable.Empty<Service>();
    }

    public async Task<(IEnumerable<Service> Items, int TotalCount)> GetPagedAsync(string term, int pageNumber, int pageSize, CancellationToken ct = default) => await _inner.GetPagedAsync(term, pageNumber, pageSize, ct);

    public async Task<bool> UpdateAsync(Service service, CancellationToken ct = default)
    {
        var updated = await _inner.UpdateAsync(service, ct);
        
        if (updated)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.ServiceById(service.Id), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesByCategoryId(service.ServiceCategoryId), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesBySpecializationId(service.SpecializationId), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesAll, ct);
            });
        }
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var serviceToDelete = await _inner.GetByIdAsync(id, ct);
        
        var deleted = await _inner.DeleteAsync(id, ct);
        if (deleted)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.ServiceById(id), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesAll, ct);

                if (serviceToDelete is not null)
                {
                    await _distributedCache.RemoveAsync(CacheKeys.ServicesByCategoryId(serviceToDelete.ServiceCategoryId), ct);
                    await _distributedCache.RemoveAsync(CacheKeys.ServicesBySpecializationId(serviceToDelete.SpecializationId), ct);
                }
            });
        }
        return deleted;
    }

    public async Task<bool> AddAsync(Service service, CancellationToken ct = default)
    {
        var added = await _inner.AddAsync(service, ct);
        
        if (added)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.ServicesByCategoryId(service.ServiceCategoryId), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesBySpecializationId(service.SpecializationId), ct);
                await _distributedCache.RemoveAsync(CacheKeys.ServicesAll, ct);
            });
        }
        return added;
    }

    public Task<IEnumerable<Service>> SearchByTerm(string term, CancellationToken ct = default) => _inner.SearchByTerm(term, ct);
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _inner.ExistsAsync(id, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) => _inner.ExistsByNameAsync(name, ct);
    public Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default) => _inner.ExistsByNameExceptIdAsync(id, name, ct);
}