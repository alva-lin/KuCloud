using KuCloud.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage;

namespace KuCloud.Infrastructure.Data;

public class EfTransaction(IDbContextTransaction transaction) : ITransaction
{
    public Guid TransactionId => transaction.TransactionId;

    public void Commit()
    {
        transaction.Commit();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    public void Rollback()
    {
        transaction.Rollback();
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        transaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }
}