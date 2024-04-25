using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record RenameFolderCommand(long Id, string Name) : ICommand<Result>;

public sealed class RenameFolderHandler(ILogger<RenameFolderHandler> logger, IRepository<Folder> repos)
    : ICommandHandler<RenameFolderCommand, Result>
{
    public async Task<Result> Handle(RenameFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RenameFolderCommand)} {request}");

        var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id, includeChildren: true), ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.Id);
            return Result.NotFound("Folder not found");
        }

        if (folder.Name == request.Name)
        {
            logger.LogWarning("Folder name is the same");
            return Result.Success();
        }

        if (folder.Parent is not null &&
            folder.Parent.Children.Any(e => e is Folder && e.Id != request.Id && e.Name == request.Name))
        {
            logger.LogWarning("Folder name already exists");
            return Result.Conflict("Folder name already exists");
        }

        var originalName = folder.Name;

        folder.Name = request.Name;

        await repos.UpdateAsync(folder, ct);

        logger.LogInformation("Rename folder [{Id}] {OriginalName} -> {Name}", folder.Id, originalName, folder.Name);

        return Result.Success();
    }
}
