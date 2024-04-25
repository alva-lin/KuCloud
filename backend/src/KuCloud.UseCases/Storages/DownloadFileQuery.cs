using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Interfaces;

namespace KuCloud.UseCases.Storages;

public sealed record DownloadFileQuery(long FileId) : IQuery<Result<DownloadFileResult>>;

public sealed record DownloadFileResult(Stream Content, string Name, string ContentType, long Size, DateTime? LastModified);

public sealed class DownloadFileHandler(
    ILogger<DownloadFileHandler> logger,
    IReadRepository<FileNode> repos,
    IFileService fileService
) : IQueryHandler<DownloadFileQuery, Result<DownloadFileResult>>
{
    public async Task<Result<DownloadFileResult>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DownloadFileQuery)} {request}");

        var file = await repos.SingleOrDefaultAsync(new SingleFileById(request.FileId), cancellationToken);
        if (file is null)
        {
            logger.LogWarning("File [{Id}] not found", request.FileId);
            return Result.NotFound("File not found");
        }

        var stream = await fileService.DownloadAsync(file.Path, cancellationToken);

        logger.LogInformation("Download file[{Id}] {Path}", file.Id, file.Path);

        var lastModified = file.AuditInfo.ModifiedTime ?? file.AuditInfo.CreationTime;
        return new DownloadFileResult( stream, file.Name, file.ContentType, file.Size, lastModified);
    }
}
