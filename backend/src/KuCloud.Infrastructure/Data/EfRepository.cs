using Ardalis.Specification.EntityFrameworkCore;

namespace KuCloud.Infrastructure.Data;

public class EfRepository<T>(AppDbContext dbContext) : RepositoryBase<T>(dbContext), IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot;
