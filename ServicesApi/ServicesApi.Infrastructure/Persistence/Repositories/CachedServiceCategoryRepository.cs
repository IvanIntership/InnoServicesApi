using Microsoft.Extensions.Caching.Memory;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;
using ServicesApi.Infrastructure.Persistence.Constants;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class CachedServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly IServiceCategoryRepository _inner;
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public CachedServiceCategoryRepository(IServiceCategoryRepository inner, IMemoryCache memoryCache)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }
    public async Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.CategoryById(id);

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var category = await _inner.GetByIdAsync(id, ct);

            if (category is null)
            {
                entry.SetAbsoluteExpiration(TimeSpan.Zero);
                return null;
            }

            entry.SetAbsoluteExpiration(CacheDuration);
            return category;
        });
    }

    public Task<IEnumerable<ServiceCategory>> SearchByTerm(string term, CancellationToken ct = default) => _inner.SearchByTerm(term, ct);

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync(CancellationToken ct = default)
    {
        string cacheKey = CacheKeys.CategoriesAll;

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _inner.GetAllAsync(ct);
        }) ?? Enumerable.Empty<ServiceCategory>();
    }

    public async Task<bool> UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        var updated = await _inner.UpdateAsync(serviceCategory, ct);
        if (updated)
        {
            _memoryCache.Remove(CacheKeys.CategoryById(serviceCategory.Id));
            _memoryCache.Remove(CacheKeys.CategoriesAll);
        }
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var deleted = await _inner.DeleteAsync(id, ct);
        if (deleted)
        {
            _memoryCache.Remove(CacheKeys.CategoryById(id));
            _memoryCache.Remove(CacheKeys.CategoriesAll);
        }
        return deleted;
    }

    public async Task<bool> AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        var added = await _inner.AddAsync(serviceCategory, ct);
        if(added)
        {
            _memoryCache.Remove(CacheKeys.CategoriesAll);
        }
        return added;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _inner.ExistsAsync(id, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) => _inner.ExistsByNameAsync(name, ct);
    public Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default) => _inner.ExistsByNameExceptIdAsync(id, name, ct);
    public Task<bool> HasAssociatedServicesAsync(Guid id, CancellationToken ct = default) => _inner.HasAssociatedServicesAsync(id, ct);
}