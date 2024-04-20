using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;

namespace KuCloud.UseCases.Storages.Folders;

public record RenameFolderCommand(long Id, string Name) : ICommand<Result>;

public sealed class RenameFolderHandler(ILogger<RenameFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<RenameFolderCommand, Result>
{
    public async Task<Result> Handle(RenameFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id), cancellationToken);
        if (folder is null)
        {
            return Result.NotFound("Folder not found");
        }

        var originalName = folder.Name;

        folder.Name = request.Name;

        // BUG: Check if the folder with the same name already exists
        // BUG: rename all children

        await repos.UpdateAsync(folder, cancellationToken);

        logger.LogDebug("Rename folder [{Id}] {OriginalName} -> {Name}", folder.Id, originalName, folder.Name);

        return Result.Success();
    }
}
