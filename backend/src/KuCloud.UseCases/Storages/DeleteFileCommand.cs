using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record DeleteFileCommand(long Id) : ICommand<Result>;

public sealed class DeleteFileHandler(
    ILogger<DeleteFileHandler> logger,
    IRepository<FileNode> fileRepos
) : ICommandHandler<DeleteFileCommand, Result>
{
    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DeleteFileCommand)} {request}");

        var spec = new SingleFileById(request.Id, includeParent: true);
        var file = await fileRepos.SingleOrDefaultAsync(spec, cancellationToken);
        if (file is null)
        {
            logger.LogWarning("File [{Id}] not found", request.Id);
            return Result.NotFound("File not found");
        }

        file.SetParent(null);

        await fileRepos.DeleteAsync(file, cancellationToken);

        logger.LogInformation("Delete file [{Id}] {Name}", file.Id, file.Name);

        return Result.Success();
    }
}
