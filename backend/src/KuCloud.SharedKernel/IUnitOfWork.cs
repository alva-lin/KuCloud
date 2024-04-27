namespace KuCloud.SharedKernel;

public interface IUnitOfWork : IDisposable
{
    ITransaction BeginTransaction();

    ITransaction? GetCurrentTransaction(bool createIfNotExists = false);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void Commit();

    Task SaveAndCommitAsync(CancellationToken cancellationToken);

    void Rollback();
}
