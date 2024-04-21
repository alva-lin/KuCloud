using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record CreateFolderCommand(string Name, long? ParentId) : ICommand<Result<Folder>>;

public sealed class CreateFolderHandler(ILogger<CreateFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<CreateFolderCommand, Result<Folder>>
{
    public async Task<Result<Folder>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(CreateFolderCommand)} {request}");

        Folder? parent = null;
        if (request.ParentId is not null)
        {
            parent = await repos.SingleOrDefaultAsync(new SingleFolderById(request.ParentId.Value), cancellationToken);
            if (parent is null)
            {
                logger.LogWarning("Parent folder [{Id}] not found", request.ParentId);
                return Result.NotFound("Parent folder not found");
            }

            if (parent.Children.Any(e => e.Name == request.Name))
            {
                logger.LogWarning("Folder with the same name already exists");
                return Result.Conflict("Folder with the same name already exists");
            }
        }

        var folder = new Folder(request.Name, parent);

        folder = await repos.AddAsync(folder, cancellationToken);

        logger.LogInformation("Create folder [{Id}] {Name}", folder.Id, folder.Name);

        return folder;
    }
}
