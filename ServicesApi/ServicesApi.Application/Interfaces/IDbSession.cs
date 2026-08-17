using System.Data;

namespace ServicesApi.Application.Interfaces;

public interface IDbSession : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    IDbTransaction BeginTransaction();
    Task CommitAsync(CancellationToken ct = default);
    void Rollback();

    void RegisterPostCommitAction(Func<Task> action);
}