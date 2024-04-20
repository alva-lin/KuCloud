using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;

namespace KuCloud.UseCases.Storages.Folders;

public record CreateFolderCommand(string Name, long? ParentId) : ICommand<Result<Folder>>;

public sealed class CreateFolderHandler(ILogger<CreateFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<CreateFolderCommand, Result<Folder>>
{
    public async Task<Result<Folder>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        Folder? parent = null;
        if (request.ParentId is not null)
        {
            parent = await repos.SingleOrDefaultAsync(new SingleFolderById(request.ParentId.Value), cancellationToken);
            if (parent is null)
            {
                return Result.NotFound("Parent folder not found");
            }

            if (parent.Children.Any(e => e.Name == request.Name))
            {
                return Result.Conflict("Folder with the same name already exists");
            }
        }

        var folder = new Folder(request.Name, parent);

        folder = await repos.AddAsync(folder, cancellationToken);

        logger.LogDebug("Create folder [{Id}] {Name}", folder.Id, folder.Name);

        return folder;
    }
}
