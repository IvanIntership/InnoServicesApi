using Microsoft.Extensions.Caching.Memory;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;

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
        string cacheKey = $"category-{id}";

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
        string cacheKey = "categories-all";

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _inner.GetAllAsync(ct);
        }) ?? Enumerable.Empty<ServiceCategory>();
    }

    public async Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(serviceCategory, ct);
        _memoryCache.Remove($"category-{serviceCategory.Id}");
        _memoryCache.Remove("categories-all");
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _inner.DeleteAsync(id, ct);
        _memoryCache.Remove($"category-{id}");
        _memoryCache.Remove("categories-all");
    }

    public async Task AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        await _inner.AddAsync(serviceCategory, ct);
        _memoryCache.Remove("categories-all");
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _inner.ExistsAsync(id, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) => _inner.ExistsByNameAsync(name, ct);
    public Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default) => _inner.ExistsByNameExceptIdAsync(id, name, ct);
    public Task<bool> HasAssociatedServicesAsync(Guid id, CancellationToken ct = default) => _inner.HasAssociatedServicesAsync(id, ct);
}