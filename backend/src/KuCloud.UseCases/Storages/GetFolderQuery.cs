using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record GetFolderQuery(long Id) : IQuery<Result<Folder>>;

public sealed class GetFolderHandler(IRepository<Folder> repos) : IQueryHandler<GetFolderQuery, Result<Folder>>
{
    public async Task<Result<Folder>> Handle(GetFolderQuery request, CancellationToken cancellationToken)
    {
        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id, readOnly: true), cancellationToken);
        if (folder is null)
        {
            return Result<Folder>.NotFound("Folder not found");
        }

        return Result<Folder>.Success(folder);
    }
}
