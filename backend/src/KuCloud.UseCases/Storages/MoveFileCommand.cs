using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

/// <summary>
///     Move files to new parent folder.
/// </summary>
/// <remarks>
///     原则上，只批量移动同一个文件夹下的文件，不支持跨文件将多选文件移动到新的文件夹下。
/// </remarks>
/// <param name="FileIds"></param>
/// <param name="FolderId"></param>
public record MoveFileCommand(long[] FileIds, long FolderId) : ICommand<Result>;

public sealed class MoveFileHandler(
    ILogger<MoveFileHandler> logger,
    IRepository<Folder> folderRepository,
    IRepository<FileNode> fileRepository
) : ICommandHandler<MoveFileCommand, Result>
{
    public async Task<Result> Handle(MoveFileCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(MoveFileCommand)} {request}");

        var specForFiles = new MultipleFilesById(request.FileIds);
        var files = await fileRepository.ListAsync(specForFiles, ct);
        if (files.Count != request.FileIds.Length)
        {
            logger.LogWarning("Some file not found");
            return Result.NotFound("Some file not found");
        }

        if (files.Select(e => e.Parent).Distinct().Count() > 1)
        {
            logger.LogWarning("Cannot move files from different folders");
            return Result.Conflict("Cannot move files from different folders");
        }

        var oldFolder = files.First().Parent;
        if (oldFolder?.Id == request.FolderId)
        {
            logger.LogWarning("Cannot move files to the same folder");
            return Result.Conflict("Cannot move files to the same folder");
        }

        var specForFolder = new SingleFolderById(request.FolderId);
        var folder = await folderRepository.SingleOrDefaultAsync(specForFolder, ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.FolderId);
            return Result.NotFound("Folder not found");
        }

        foreach (var file in files)
        {
            file.SetParent(folder);
        }

        await folderRepository.UpdateAsync(folder, ct);

        return Result.Success();
    }
}
