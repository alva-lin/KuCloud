using MediatR;
using Microsoft.Extensions.Logging;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class FolderDeletedEvent(long folderId) : DomainEventBase
{
    public long FolderId { get; set; } = folderId;
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

        folder.SetParent(null);

        logger.LogInformation("Folder [{Id}] clear all ancestor relations", notification.FolderId);

        await Task.CompletedTask;
    }
}
