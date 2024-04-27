using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

/// <summary>
///     Move folder to new parent folder.
/// </summary>
/// <param name="Id"></param>
/// <param name="NewParentId"></param>
/// <param name="IncludeDeleted">if set true, even folder is deleted, it also moves folder to new parent and restore it, or move failed.</param>
public sealed record MoveFolderCommand(long Id, long NewParentId, bool IncludeDeleted = false) : ICommand<Result>;

/// <summary>
///     将文件夹移动到新的父文件夹，如果文件夹已经在新的父文件夹下，则不做任何操作。
/// </summary>
/// <remarks>
///     将被删除的文件夹视为普通文件夹，可以移动到新的父文件夹下，同时恢复文件夹。
/// </remarks>
public sealed class MoveFolderHandler(ILogger<MoveFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<MoveFolderCommand, Result>
{
    public async Task<Result> Handle(MoveFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(MoveFolderCommand)} {request}");

        var folder = await repos.SingleOrDefaultAsync(
            new SingleFolderById(request.Id, includeDeleted: request.IncludeDeleted),
            cancellationToken: ct
        );
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.Id);
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
        folder = await repos.SingleOrDefaultAsync(
            new SingleFolderForAllInfo(request.Id, includeDeleted: request.IncludeDeleted),
            cancellationToken: ct
        );
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.Id);
            return Result.NotFound("Folder not found");
        }

        if (folder.IsAncestorOf(newParent))
        {
            logger.LogWarning("Cannot move folder to its descendant");
            return Result.Conflict("Cannot move folder to its descendant");
        }

        folder.SetParent(newParent);

        // restore folder if it is deleted
        if (request.IncludeDeleted && folder.AuditInfo.IsDelete)
        {
            folder.AuditInfo.Restore();
        }

        await repos.UpdateAsync(folder, ct);

        logger.LogInformation("Move folder [{Id}] to parent [{ParentId}]", folder.Id, newParent.Id);

        return Result.Success();
    }
}
