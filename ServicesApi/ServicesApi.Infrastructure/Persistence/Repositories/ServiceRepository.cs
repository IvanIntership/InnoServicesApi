using Dapper;
using ServicesApi.Domain.Entities;
using ServicesApi.Domain.Interfaces;

namespace ServicesApi.Infrastructure.Persistence.Repositories;

public sealed class ServiceRepository : IServiceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ServiceRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM services 
                           WHERE id=@Id AND is_active = true
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Service>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IEnumerable<Service>> GetByCategoryId(Guid categoryId, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM services 
                           WHERE service_category_id=@Id AND is_active = true
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Service>(new CommandDefinition(sql, new { Id = categoryId }, cancellationToken: ct));
    }

    public async Task<IEnumerable<Service>> GetBySpecializationId(Guid specializationId, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT *
                           FROM services 
                           WHERE specialization_id=@Id AND is_active = true
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Service>(new CommandDefinition(sql, new { Id = specializationId }, cancellationToken: ct));
    }

    public async Task<IEnumerable<Service>> SearchByTerm(string term, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT *  
                           FROM services
                           WHERE name ILIKE '%' || @Term || '%' AND is_active = true;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Service>(new CommandDefinition(sql, new { Term = term }, cancellationToken: ct));
    }

    public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT * 
                           FROM services
                           WHERE is_active = true;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Service>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task UpdateAsync(Service service, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE services
                           SET specialization_id = @SpecializationId,
                               service_category_id = @ServiceCategoryId,
                               name = @Name,
                               price = @Price,
                               is_active = @IsActive
                           WHERE id = @Id AND is_active = true;
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, service, cancellationToken: ct));
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE services
                           SET is_active = false
                           WHERE id = @Id;
                           """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task AddAsync(Service service, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO services (id, specialization_id, service_category_id, name, price, is_active) 
                           VALUES (@Id, @SpecializationId, @ServiceCategoryId, @Name, @Price, @IsActive);
                           """;
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, service, cancellationToken: ct));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM services 
                           WHERE id = @Id AND is_active = true;
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT 1 
                           FROM services 
                           WHERE LOWER(name) = LOWER(@Name) AND is_active = true 
                           LIMIT 1;
                           """;
        using var connection = _connectionFactory.CreateConnection();
        
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name }, cancellationToken: ct));
    }
}