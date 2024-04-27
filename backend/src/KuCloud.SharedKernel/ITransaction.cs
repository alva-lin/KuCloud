namespace KuCloud.SharedKernel;

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Guid TransactionId { get; }

    void Commit();

    Task CommitAsync(CancellationToken cancellationToken = default);


    void Rollback();

    Task RollbackAsync(CancellationToken cancellationToken = default);

    void CreateSavepoint(string name)
        => throw new NotSupportedException();

    Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    void RollbackToSavepoint(string name)
        => throw new NotSupportedException();

    Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    void ReleaseSavepoint(string name) { }

    Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    bool SupportsSavepoints
        => false;
}