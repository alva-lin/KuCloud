using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;

namespace KuCloud.UseCases.Storages.Folders;

public record GetFolderQuery(long Id) : IQuery<Result<Folder>>;

public class GetFolderHandler(IRepository<Folder> repos) : IQueryHandler<GetFolderQuery, Result<Folder>>
{
    public async Task<Result<Folder>> Handle(GetFolderQuery request, CancellationToken cancellationToken)
    {
        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id), cancellationToken);
        if (folder is null)
        {
            return Result<Folder>.NotFound("Folder not found");
        }

        return Result<Folder>.Success(folder);
    }
}
