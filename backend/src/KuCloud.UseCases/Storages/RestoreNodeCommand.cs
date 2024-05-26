using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

/// <summary>
///     restore multiple nodes and move them to a new parent folder
/// </summary>
/// <param name="Ids">list of ids of nodes to be restored</param>
/// <param name="FolderId">new parent folder id</param>
public sealed record RestoreNodeCommand(long[] Ids, long FolderId) : ICommand<Result>;

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
            var moveCommand = new MoveNodeCommand(request.Ids, request.FolderId);
            var result = await mediator.Send(moveCommand, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation(
                "Nodes [{Ids}] restored and moved to parent [{ParentId}]",
                request.Ids,
                request.FolderId
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
