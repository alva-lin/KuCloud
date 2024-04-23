using MediatR;
using Microsoft.Extensions.Logging;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class FolderDeletedEvent(long folderId, long[] childrenIds) : DomainEventBase
{
    public long FolderId { get; set; } = folderId;

    public long[] ChildrenIds { get; set; } = childrenIds;
}

public class FolderDeletedHandler(ILogger<FolderDeletedHandler> logger, IRepository<Folder> repos)
    : INotificationHandler<FolderDeletedEvent>
{
    public async Task Handle(FolderDeletedEvent notification, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(FolderDeletedEvent)} {notification}");

        var spec = new SingleFolderById(notification.FolderId, includeDeleted: true);
        var folder = await repos.SingleOrDefaultAsync(spec, ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", notification.FolderId);
            return;
        }

        if (!folder.AuditInfo.IsDelete)
        {
            logger.LogWarning("Folder [{Id}] is not deleted", notification.FolderId);
        }

        // 在执行 Delete 时，一级子节点的 parentId 会被置为 null，需要恢复节点关系
        var specForChildren = new MultipleFoldsById(notification.ChildrenIds,
            includeDeleted: true, includeDescendant: true);
        var children = await repos.ListAsync(specForChildren, ct);
        foreach (var child in children)
        {
            child.SetParent(folder);
        }

        await repos.UpdateAsync(folder, ct);

        logger.LogInformation("Folder [{Id}] reset all parent-child relation", notification.FolderId);
    }
}
