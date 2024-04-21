using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

/// <summary>
///     查询文件夹的所有信息，包括祖先、后代、子节点
///     FIXME: 性能问题 查出祖先链，并将祖先的所有后代关系也查出来
/// </summary>
public sealed class SingleFolderForAllInfo : Specification<Folder>, ISingleResultSpecification<Folder>
{
    public SingleFolderForAllInfo(long id, bool readOnly = false)
    {
        Query.Where(x => x.Id == id);

        Query.Include(e => e.AncestorRelations).ThenInclude(e => e.Ancestor).ThenInclude(e => e.DescendantRelations);
        Query.Include(e => e.DescendantRelations).ThenInclude(e => e.Descendant);
        Query.Include(e => e.Children);

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}