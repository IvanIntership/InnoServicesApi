namespace ServicesApi.Infrastructure.Persistence.Constants;

public static class CacheKeys
{
    public const string CategoriesAll = "categories-all";
    public static string CategoryById(Guid id) => $"category-{id}";
    
    public const string ServicesAll = "services-all";
    public static string ServiceById(Guid id) => $"service-{id}";
    public static string ServicesByCategoryId(Guid categoryId) => $"services-category-{categoryId}";
    public static string ServicesBySpecializationId(Guid specializationId) => $"services-specialization-{specializationId}";
}