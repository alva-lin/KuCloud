using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.SharedKernel;

namespace KuCloud.UseCases.Storages;

public sealed record RestoreFolderCommand(long Id, long ParentId) : ICommand<Result>;

public sealed class RestoreFolderHandler(
    ILogger<RestoreFolderHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<Folder> repos,
    IMediator mediator)
    : ICommandHandler<RestoreFolderCommand, Result>
{
    public async Task<Result> Handle(RestoreFolderCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RestoreFolderCommand)} {request}");

        await using var transaction = unitOfWork.BeginTransaction();
        try
        {
            // 先恢复文件夹
            var folder = await repos.SingleOrDefaultAsync(new SingleFolderById(request.Id, includeDeleted: true), ct);
            if (folder is null)
            {
                logger.LogWarning("Folder [{Id}] not found", request.Id);
                return Result.NotFound("Folder not found.");
            }

            folder.AuditInfo.Restore();
            await repos.UpdateAsync(folder, ct);

            // 移动到指定文件夹
            var moveCommand = new MoveFolderCommand(request.Id, request.ParentId, IncludeDeleted: true);
            var result = await mediator.Send(moveCommand, ct);

            await transaction.CommitAsync(ct);
            logger.LogInformation("Folder [{Id}] restored and moved to parent [{ParentId}]", request.Id, request.ParentId);

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
