using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.SharedKernel;

namespace KuCloud.UseCases.Storages;

public sealed record RestoreNodeCommand(long[] Ids, long NewParentId) : ICommand<Result>;

public sealed class RestoreNodeHandler(
    ILogger<RestoreNodeHandler> logger,
    IUnitOfWork unitOfWork,
    IRepository<StorageNode> repos,
    IMediator mediator
) : ICommandHandler<RestoreNodeCommand, Result>
{
    public async Task<Result> Handle(RestoreNodeCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RestoreNodeCommand)} {request}");

        await using var transaction = unitOfWork.BeginTransaction();
        try
        {
            // 先恢复节点
            var nodes = await repos.ListAsync(new MultipleNodesById(request.Ids, includeDeleted: true),
                cancellationToken);
            if (nodes.Count == 0)
            {
                logger.LogWarning("Nodes [{Ids}] not found", request.Ids);
                return Result.NotFound("Nodes not found");
            }

            foreach (var node in nodes)
            {
                node.AuditInfo.Restore();
            }

            await repos.UpdateRangeAsync(nodes, cancellationToken);

            // 移动到指定文件夹
            var moveCommand = new MoveNodeCommand(request.Ids, request.NewParentId);
            var result = await mediator.Send(moveCommand, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation(
                "Nodes [{Ids}] restored and moved to parent [{ParentId}]",
                request.Ids,
                request.NewParentId
            );

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error handling {Request}", request);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Error(e.Message);
        }
    }
}
