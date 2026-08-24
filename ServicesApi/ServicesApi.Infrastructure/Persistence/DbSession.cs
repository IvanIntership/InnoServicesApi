using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ServicesApi.Application.Interfaces;

namespace ServicesApi.Infrastructure.Persistence;

public sealed class DbSession : IDbSession
{
    private readonly NpgsqlConnection _connection;
    private readonly List<Func<Task>> _postCommitActions = [];
    private bool _disposed;

    public DbSession(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        _connection = new NpgsqlConnection(connectionString);
    }

    public IDbConnection Connection
    {
        get
        {
            EnsureConnectionIsOpen();
            return _connection;
        }
    }

    public IDbTransaction? Transaction { get; private set; }

    public IDbTransaction BeginTransaction()
    {
        EnsureConnectionIsOpen();
        Transaction ??= _connection.BeginTransaction();
        return Transaction;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (Transaction == null) return;

        Transaction.Commit();
        Transaction.Dispose();
        Transaction = null;

        foreach (var action in _postCommitActions)
        {
            await action();
        }

        _postCommitActions.Clear();
    }

    public void Rollback()
    {
        if (Transaction == null) return;

        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;

        _postCommitActions.Clear();
    }

    public void RegisterPostCommitAction(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _postCommitActions.Add(action);
    }

    private void EnsureConnectionIsOpen()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (Transaction != null)
        {
            try { Transaction.Rollback(); } catch {  }
            Transaction.Dispose();
            Transaction = null;
        }

        _connection.Dispose();
        _postCommitActions.Clear();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (Transaction != null)
        {
            try { Transaction.Rollback(); } catch {  }
            Transaction.Dispose();
            Transaction = null;
        }

        await _connection.DisposeAsync();
        _postCommitActions.Clear();
        _disposed = true;
    }
}