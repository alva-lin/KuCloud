using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class SingleFileById : Specification<FileNode>, ISingleResultSpecification<FileNode>
{
    public SingleFileById(
        long id,
        bool readOnly = false,
        bool includeParent = true,
        bool includeDeleted = false
    )
    {
        Query.Where(e => e.Id == id);

        if (includeParent)
        {
            Query.Include(e => e.Parent).ThenInclude(e => e!.Children);
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }
        else
        {
            Query.Where(e => e.Parent != null || !e.Parent!.AuditInfo.IsDelete);
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}

public sealed class SingleNodeById : Specification<StorageNode>, ISingleResultSpecification<StorageNode>
{
    public SingleNodeById(
        long id,
        bool includeDeleted = false,
        bool includeParents = false,
        bool readOnly = false
    )
    {
        Query.Where(e => e.Id == id);

        if (includeParents)
        {
            Query.Include(e => e.Parent).ThenInclude(e => e!.Children);
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }
        else
        {
            Query.Where(e =>
                !e.AuditInfo.IsDelete
             && (e.Parent == null || !e.Parent.AuditInfo.IsDelete)
             && (e.Type != StorageType.Folder
              || (e as Folder)!.AncestorRelations.All(r => !r.Ancestor.AuditInfo.IsDelete)
                )
            );
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}
