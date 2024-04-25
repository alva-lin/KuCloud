using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

/// <summary>
///     Move files to new parent folder.
/// </summary>
/// <remarks>
///     原则上，只批量移动同一个文件夹下的文件，不支持跨文件将多选文件移动到新的文件夹下。
/// </remarks>
/// <param name="Ids"></param>
/// <param name="ParentId"></param>
/// <param name="IncludeDeleted">if set true, even file is deleted, it also moves file to new parent and restore it, or move failed.</param>
public record MoveFileCommand(long[] Ids, long ParentId, bool IncludeDeleted = false) : ICommand<Result>;

public sealed class MoveFileHandler(
    ILogger<MoveFileHandler> logger,
    IRepository<Folder> folderRepository,
    IRepository<FileNode> fileRepository
) : ICommandHandler<MoveFileCommand, Result>
{
    public async Task<Result> Handle(MoveFileCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(MoveFileCommand)} {request}");

        var specForFiles = new MultipleFilesById(request.Ids, includeDeleted: request.IncludeDeleted);
        var files = await fileRepository.ListAsync(specForFiles, ct);
        if (files.Count != request.Ids.Length)
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
        if (oldFolder?.Id == request.ParentId)
        {
            logger.LogWarning("Cannot move files to the same folder");
            return Result.Conflict("Cannot move files to the same folder");
        }

        var specForFolder = new SingleFolderById(request.ParentId);
        var folder = await folderRepository.SingleOrDefaultAsync(specForFolder, ct);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.ParentId);
            return Result.NotFound("Folder not found");
        }

        foreach (var file in files)
        {
            file.SetParent(folder);
        }

        // TODO - 是否需要拆开？另外 RestoreFile / MoveFile 也有类似的逻辑
        // Restore files if IncludeDeleted is true
        if (request.IncludeDeleted)
        {
            foreach (var file in files)
            {
                file.AuditInfo.Restore();
            }
        }

        await folderRepository.UpdateAsync(folder, ct);

        return Result.Success();
    }
}
