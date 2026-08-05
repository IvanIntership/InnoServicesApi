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
                           SELECT id, name, duration 
                           FROM service_categories 
                           WHERE id=@Id;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ServiceCategory>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IEnumerable<ServiceCategory>> SearchByTerm(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name, duration 
                           FROM service_categories 
                           WHERE name ILIKE '%' || @Term || '%';
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ServiceCategory>(new CommandDefinition(sql, new { Term = name }, cancellationToken: ct));
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name, duration 
                           FROM service_categories;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ServiceCategory>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task UpdateAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE service_categories 
                           SET name = @Name, duration = @Duration 
                           WHERE id = @Id;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, serviceCategory, cancellationToken: ct));
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           DELETE 
                           FROM service_categories 
                           WHERE id=@Id;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task AddAsync(ServiceCategory serviceCategory, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO service_categories (id, name, duration) 
                           VALUES (@Id, @Name, @Duration);
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, serviceCategory, cancellationToken: ct));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT EXISTS
                               (SELECT 1 
                                FROM service_categories 
                                WHERE id=@Id
                                );
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT EXISTS(
                           SELECT 1 
                           FROM service_categories 
                           WHERE LOWER(name)=LOWER(@Name)
                           );
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name }, cancellationToken: ct));
    }
}