using KuCloud.Core.Interfaces;

namespace KuCloud.UseCases.Storages;

public record UploadFileCommand(Stream Stream)
    : ICommand<Result<string>>;

public sealed class UploadFileHandler(
    ILogger<UploadFileHandler> logger,
    IFileService fileService
) : ICommandHandler<UploadFileCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(UploadFileCommand)} {request}");

        var path = await fileService.UploadAsync(request.Stream, cancellationToken);

        logger.LogInformation("Upload file {Path}", path);

        return path;
    }
}
