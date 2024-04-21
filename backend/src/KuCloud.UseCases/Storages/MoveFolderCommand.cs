using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record MoveFolderCommand(long FolderId, long NewParentId) : ICommand<Result>;

public sealed class MoveFolderHandler(ILogger<MoveFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<MoveFolderCommand, Result>
{
    public async Task<Result> Handle(MoveFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(MoveFolderCommand)} {request}");

        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.FolderId), ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.FolderId);
            return Result.NotFound("Folder not found");
        }

        if (folder.Id == request.NewParentId)
        {
            logger.LogWarning("Cannot move folder to itself");
            return Result.Conflict("Cannot move folder to itself");
        }

        if (folder.Parent?.Id == request.NewParentId)
        {
            logger.LogWarning("Folder [{Id}] is already in parent [{ParentId}]", folder.Id, request.NewParentId);
            return Result.Success();
        }

        var specForNewParent = new SingleFolderById(request.NewParentId, includeChildren: true);
        var newParent = await repos.SingleOrDefaultAsync(specForNewParent, ct);
        if (newParent is null)
        {
            logger.LogWarning("New parent folder [{Id}] not found", request.NewParentId);
            return Result.NotFound("New parent folder not found");
        }

        // inorder to check if new parent is descendant of folder, and to set new parent
        // need to load all children of folder
        folder = await repos.SingleOrDefaultAsync(new SingleFolderForAllInfo(request.FolderId),
            ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.FolderId);
            return Result.NotFound("Folder not found");
        }

        if (folder.IsAncestorOf(newParent))
        {
            logger.LogWarning("Cannot move folder to its descendant");
            return Result.Conflict("Cannot move folder to its descendant");
        }

        if (newParent.Children.Any(e => e is Folder && e.Id != folder.Id && e.Name == folder.Name))
        {
            logger.LogWarning("Folder name already exists in new parent");
            var index = 1;
            while (newParent.Children.Any(e =>
                       e is Folder && e.Id != folder.Id && e.Name == $"{folder.Name} ({index})"))
            {
                index++;
            }

            var newName = $"{folder.Name} ({index})";
            folder.Name = newName;
            logger.LogInformation("Rename folder [{Id}] to [{Name}]", folder.Id, newName);
        }

        folder.SetParent(newParent);

        await repos.UpdateAsync(folder, ct);

        logger.LogInformation("Move folder [{Id}] to parent [{ParentId}]", folder.Id, newParent.Id);

        return Result.Success();
    }
}
