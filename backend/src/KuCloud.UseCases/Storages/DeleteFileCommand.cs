using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public record DeleteFileCommand(long FileId) : ICommand<Result>;

public sealed class DeleteFileHandler(
    ILogger<DeleteFileHandler> logger,
    IRepository<FileNode> fileRepository
) : ICommandHandler<DeleteFileCommand, Result>
{
    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DeleteFileCommand)} {request}");

        var file = await fileRepository.SingleOrDefaultAsync(new SingleFileById(request.FileId), cancellationToken);
        if (file is null)
        {
            logger.LogWarning("File [{Id}] not found", request.FileId);
            return Result.NotFound("File not found");
        }

        await fileRepository.DeleteAsync(file, cancellationToken);

        logger.LogInformation("Delete file [{Id}] {Name}", file.Id, file.Name);

        return Result.Success();
    }
}
