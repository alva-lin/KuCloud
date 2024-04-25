using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record RenameFileCommand(long FileId, string NewName) : ICommand<Result>;

public sealed class RenameFileHandler(
    ILogger<RenameFileHandler> logger,
    IRepository<FileNode> repos
) : ICommandHandler<RenameFileCommand, Result>
{
    public async Task<Result> Handle(RenameFileCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RenameFileCommand)} {request}");

        var specs = new SingleFileById(request.FileId);
        var file = await repos.SingleOrDefaultAsync(specs, ct);

        if (file is null)
        {
            logger.LogWarning("File [{Id}] not found", request.FileId);
            return Result.NotFound("File not found");
        }

        if (file.Name == request.NewName)
        {
            logger.LogWarning("New name is same as old name");
            return Result.Conflict("New name is same as old name");
        }

        var oldName = file.Name;
        var newName = request.NewName;
        var index = 1;
        while (file.Parent!.Children.Any(e => e.Type == StorageType.File && e.Id != file.Id && e.Name == newName))
        {
            newName = $"{request.NewName}_{index}";
            index++;
        }

        file.Name = newName;

        await repos.UpdateAsync(file, ct);

        logger.LogInformation("Rename file [{Id}] {OldName} -> {NewName}", file.Id, oldName, newName);

        return Result.Success();
    }
}
