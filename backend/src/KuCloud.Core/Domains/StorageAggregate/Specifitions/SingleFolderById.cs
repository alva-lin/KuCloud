using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate.Specifitions;

public sealed class SingleFolderById : Specification<Folder>, ISingleResultSpecification<Folder>
{
    public SingleFolderById(long id)
    {
        Query.Where(x => x.Id == id);

        Query.Include(e => e.Children);
    }
}
