using Microsoft.EntityFrameworkCore.Infrastructure;

namespace KuCloud.Infrastructure.Data;

public class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly DatabaseFacade _database = dbContext.Database;
    private EfTransaction? _transaction;
    private readonly object _lock = new();

    public ITransaction BeginTransaction()
    {
        var transaction = _database.BeginTransaction();
        _transaction = new EfTransaction(transaction);
        return _transaction;
    }

    public ITransaction? GetCurrentTransaction(bool createIfNotExists = false)
    {
        if (_transaction != null || !createIfNotExists) return _transaction;

        lock (_lock)
        {
            if (_transaction == null && createIfNotExists)
            {
                return BeginTransaction();
            }
        }

        return _transaction;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Commit()
    {
        _transaction?.Commit();
    }

    public async Task SaveAndCommitAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
        Commit();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
