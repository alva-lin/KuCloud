using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record GetFolderQuery(long Id) : IQuery<Result<FolderDto>>;

public sealed class GetFolderHandler(IReadRepository<Folder> repos) : IQueryHandler<GetFolderQuery, Result<FolderDto>>
{
    public async Task<Result<FolderDto>> Handle(GetFolderQuery request, CancellationToken cancellationToken)
    {
        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id, readOnly: true), cancellationToken);
        if (folder is null)
        {
            return Result<FolderDto>.NotFound("Folder not found");
        }

        return Result<FolderDto>.Success(FolderDto.Map(folder));
    }
}
