using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.SharedKernel;

namespace KuCloud.UseCases.Storages;

public sealed record RestoreFileCommand(long[] Ids, long FolderId) : ICommand<Result>;

// TODO - 单元测试，包括 RestoreFolderHandler
public sealed class RestoreFileHandler(
    ILogger<RestoreFileHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<FileNode> repos,
    IMediator mediator
) : ICommandHandler<RestoreFileCommand, Result>
{
    public async Task<Result> Handle(RestoreFileCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RestoreFileCommand)} {request}");

        await using var transaction = unitOfWork.BeginTransaction();
        try
        {
            // 先恢复文件
            var files = await repos.ListAsync(new MultipleFilesById(request.Ids, includeDeleted: true), ct);

            foreach (var file in files)
            {
                file.AuditInfo.Restore();
            }

            await repos.UpdateRangeAsync(files, ct);

            // 移动到指定文件夹
            var moveCommand = new MoveFileCommand(request.Ids, request.FolderId, IncludeDeleted: true);
            var result = await mediator.Send(moveCommand, ct);

            await transaction.CommitAsync(ct);
            logger.LogInformation("Files [{Ids}] restored and moved to folder [{FolderId}]",
                request.Ids,
                request.FolderId
            );

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error handling {Request}", request);
            await transaction.RollbackAsync(ct);
            return Result.Error(e.Message);
        }
    }
}
