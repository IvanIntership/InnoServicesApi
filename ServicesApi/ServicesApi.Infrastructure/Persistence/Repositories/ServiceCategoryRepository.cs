using Dapper;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class ServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ServiceCategoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM service_categories 
                           WHERE id=@Id;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ServiceCategory>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IEnumerable<ServiceCategory>> SearchByTerm(string term, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM service_categories 
                           WHERE name ILIKE '%' || @Term || '%';
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ServiceCategory>(new CommandDefinition(sql, new { Term = term }, cancellationToken: ct));
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM service_categories;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ServiceCategory>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<(IEnumerable<ServiceCategory> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        const string sql = """
                           SELECT COUNT(*) 
                           FROM service_categories 
                           WHERE (@SearchTerm IS NULL OR @SearchTerm = '' OR name ILIKE '%' || @SearchTerm || '%');

                           SELECT * 
                           FROM service_categories 
                           WHERE (@SearchTerm IS NULL OR @SearchTerm = '' OR name ILIKE '%' || @SearchTerm || '%')
                           ORDER BY name
                           LIMIT @PageSize OFFSET @Offset;
                           """;

        var parameters = new
        {
            SearchTerm = searchTerm?.Trim(),
            PageSize = pageSize,
            Offset = (pageNumber - 1) * pageSize
        };

        using var connection = _connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, parameters, cancellationToken: ct));

        var totalCount = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<ServiceCategory>();

        return (items, totalCount);
    }

    public async Task<bool> UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE service_categories 
                           SET name = @Name, duration = @Duration 
                           WHERE id = @Id
                           AND name IS DISTINCT FROM @Name OR duration IS DISTINCT FROM @Duration;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(sql, serviceCategory, cancellationToken: ct));
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           DELETE 
                           FROM service_categories 
                           WHERE id=@Id;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO service_categories (id, name, duration) 
                           VALUES (@Id, @Name, @Duration);
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(sql, serviceCategory, cancellationToken: ct));
        
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM service_categories 
                           WHERE id = @Id;
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM service_categories 
                           WHERE LOWER(name) = LOWER(@Name) 
                           LIMIT 1;
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name }, cancellationToken: ct));
    }

    public async Task<bool> ExistsByNameExceptIdAsync(Guid id, string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM service_categories 
                           WHERE LOWER(name) = LOWER(@Name) AND id <> @Id
                           LIMIT 1;
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name, Id = id }, cancellationToken: ct));
    }

    public async Task<bool> HasAssociatedServicesAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM services 
                           WHERE service_category_id = @CategoryId AND is_active = true
                           LIMIT 1
                           """;
        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { CategoryId = id }, cancellationToken: ct));
    }
}