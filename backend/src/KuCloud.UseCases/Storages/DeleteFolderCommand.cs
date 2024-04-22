using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record DeleteFolderCommand(long Id) : ICommand<Result>;

public class DeleteFolderHandler(ILogger<DeleteFolderHandler> logger, IRepository<Folder> repos, IMediator mediator)
    : ICommandHandler<DeleteFolderCommand, Result>
{
    public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DeleteFolderCommand)} {request}");

        var folder = await repos.SingleOrDefaultAsync(new SingleFolderForAllInfo(request.Id), ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.Id);
            return Result.NotFound("Folder not found");
        }

        var childrenIds = folder.Children.Select(e => e.Id).ToArray();
        folder.SetParent(null);

        await repos.DeleteAsync(folder, ct);

        await mediator.Publish(new FolderDeletedEvent(folder.Id, childrenIds), ct);

        logger.LogInformation("Delete folder [{Id}] {Name}", folder.Id, folder.Name);

        return Result.Success();
    }
}
