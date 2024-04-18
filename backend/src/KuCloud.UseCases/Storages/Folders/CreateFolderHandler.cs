using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Domains.StorageAggregate.Specifitions;
using Microsoft.Extensions.Logging;

namespace KuCloud.UseCases.Storages.Folders;

public record CreateFolderCommand(string Name, long? ParentId) : ICommand<Result<long>>;

public sealed class CreateFolderHandler(ILogger<CreateFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<CreateFolderCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        Folder? parent = null;
        if (request.ParentId is not null)
        {
            parent = await repos.SingleOrDefaultAsync(new SingleFolderById(request.ParentId.Value), cancellationToken);
            if (parent is null)
            {
                return Result.NotFound("Parent folder not found");
            }
        }

        var folder = new Folder(request.Name, parent);

        await repos.AddAsync(folder, cancellationToken);

        logger.LogDebug("Create folder [{Id}] {Name}", folder.Id, folder.Name);

        return folder.Id;
    }
}
