using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;

namespace KuCloud.UseCases.Storages.Folders;

public record DeleteFolderCommand(long Id) : ICommand<Result>;

public class DeleteFolderHandler(ILogger<DeleteFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<DeleteFolderCommand, Result>
{
    public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id), cancellationToken);
        if (folder is null)
        {
            return Result.NotFound("Folder not found");
        }

        // TODO: is need delete all children?

        await repos.DeleteAsync(folder, cancellationToken);

        logger.LogDebug("Delete folder [{Id}] {Name}", folder.Id, folder.Name);

        return Result.Success();
    }
}
