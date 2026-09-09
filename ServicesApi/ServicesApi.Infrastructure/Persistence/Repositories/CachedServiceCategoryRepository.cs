using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ServicesApi.Application.Interfaces;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;
using ServicesApi.Infrastructure.Persistence.Constants;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class CachedServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly IServiceCategoryRepository _inner;
    private readonly IDistributedCache _distributedCache;
    private readonly IDbSession _dbSession;
    
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public CachedServiceCategoryRepository(IServiceCategoryRepository inner, IDistributedCache distributedCache, IDbSession dbSession)
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

    public async Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.CategoryById(id);
        return await GetOrCreateAsync(cacheKey, () => _inner.GetByIdAsync(id, ct), ct);
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync(CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.CategoriesAll;
        var categories = await GetOrCreateAsync(cacheKey, () => _inner.GetAllAsync(ct), ct);
        return categories ?? Enumerable.Empty<ServiceCategory>();
    }

    public async Task<(IEnumerable<ServiceCategory> Items, int TotalCount)> GetPagedAsync(string term, int pageNumber, int pageSize, CancellationToken ct = default) => await _inner.GetPagedAsync(term, pageNumber, pageSize, ct);

    public async Task<bool> UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        var updated = await _inner.UpdateAsync(serviceCategory, ct);
        if (updated)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.CategoryById(serviceCategory.Id), ct);
                await _distributedCache.RemoveAsync(CacheKeys.CategoriesAll, ct);
            });
        }
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var deleted = await _inner.DeleteAsync(id, ct);
        if (deleted)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.CategoryById(id), ct);
                await _distributedCache.RemoveAsync(CacheKeys.CategoriesAll, ct);
            });
        }
        return deleted;
    }

    public async Task<bool> AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        var added = await _inner.AddAsync(serviceCategory, ct);
        if (added)
        {
            _dbSession.RegisterPostCommitAction(async () =>
            {
                await _distributedCache.RemoveAsync(CacheKeys.CategoriesAll, ct);
            });
        }
        return added;
    }
    
    public Task<IEnumerable<ServiceCategory>> SearchByTerm(string term, CancellationToken ct = default) => _inner.SearchByTerm(term, ct);
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _inner.ExistsAsync(id, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) => _inner.ExistsByNameAsync(name, ct);
    public Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default) => _inner.ExistsByNameExceptIdAsync(id, name, ct);
    public Task<bool> HasAssociatedServicesAsync(Guid id, CancellationToken ct = default) => _inner.HasAssociatedServicesAsync(id, ct);
}