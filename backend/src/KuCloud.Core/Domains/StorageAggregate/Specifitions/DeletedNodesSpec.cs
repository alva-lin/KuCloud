using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class DeletedNodesSpec : Specification<StorageNode>
{
    public DeletedNodesSpec(int page, int pageSize, string? keyword)
    {
        Query.IgnoreQueryFilters();

        Query.Where(e => e.AuditInfo.IsDelete);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            Query.Where(e => e.Name.Contains(keyword));
        }

        Query.OrderByDescending(e => e.AuditInfo.DeletionTime);

        Query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
