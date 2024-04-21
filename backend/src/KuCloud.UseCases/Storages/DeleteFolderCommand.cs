using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record DeleteFolderCommand(long Id) : ICommand<Result>;

public class DeleteFolderHandler(ILogger<DeleteFolderHandler> logger, IRepository<Folder> repos, IMediator mediator)
    : ICommandHandler<DeleteFolderCommand, Result>
{
    public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DeleteFolderCommand)} {request}");

        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id), cancellationToken);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.Id);
            return Result.NotFound("Folder not found");
        }

        await repos.DeleteAsync(folder, cancellationToken);

        await mediator.Publish(new FolderDeletedEvent(folder.Id), cancellationToken);

        logger.LogInformation("Delete folder [{Id}] {Name}", folder.Id, folder.Name);

        return Result.Success();
    }
}
