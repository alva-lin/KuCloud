using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Interfaces;

namespace KuCloud.UseCases.Storages;

// TODO - 添加文件应该使用别的方式，而不是使用 path 来定位
public sealed record AddFileCommand(long FolderId, string Path, string FileName) : ICommand<Result<long>>;

public sealed class AddFileHandler(
    ILogger<AddFileHandler> logger,
    IReadRepository<Folder> folderRepos,
    IRepository<FileNode> fileRepos,
    IFileService fileService
) : ICommandHandler<AddFileCommand, Result<long>>
{
    public async Task<Result<long>> Handle(AddFileCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(AddFileCommand)} {request}");

        if (!await fileService.ExistsAsync(request.Path, cancellationToken))
        {
            logger.LogWarning("File [{Path}] not found", request.Path);
            return Result.NotFound("File not found");
        }

        var folder =
            await folderRepos.SingleOrDefaultAsync(new SingleFolderById(request.FolderId), cancellationToken);
        if (folder is null)
        {
            logger.LogWarning("Folder [{Id}] not found", request.FolderId);
            return Result.NotFound("Folder not found");
        }

        var fileSize = await fileService.GetSizeAsync(request.Path, cancellationToken);

        var file = new FileNode(folder, request.FileName, request.Path, fileSize);

        file = await fileRepos.AddAsync(file, cancellationToken);

        logger.LogInformation("Add file [{Id}] {Name}", file.Id, file.Name);

        return file.Id;
    }
}
