using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record DeleteNodeCommand(long[] Ids) : ICommand<Result>;

public sealed class DeleteNodeHandler(
    ILogger<DeleteNodeHandler> logger,
    IRepository<StorageNode> repos
) : ICommandHandler<DeleteNodeCommand, Result>
{
    public async Task<Result> Handle(DeleteNodeCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(DeleteNodeCommand)} {request}");

        var spec = new MultipleNodesById(request.Ids, includeDeleted: true, includeFolderNesting: true);
        var nodes = await repos.ListAsync(spec, ct);
        if (nodes.Count <= 0) return Result.Success();

        var turnToTrash = nodes.Where(e => !e.AuditInfo.IsDelete).ToList();
        var actualDelete = nodes.Where(e => e.AuditInfo.IsDelete).ToList();

        foreach (var node in turnToTrash)
        {
            node.SetParent(null);
            node.AuditInfo.SetDeleteInfo(DateTime.UtcNow);
        }

        await repos.UpdateRangeAsync(turnToTrash, ct);
        await repos.DeleteRangeAsync(actualDelete, ct);

        logger.LogInformation("Turn to trash nodes [{Ids}]", turnToTrash.Select(e => e.Id));
        logger.LogInformation("Delete nodes [{Ids}]", actualDelete.Select(e => e.Id));

        return Result.Success();
    }
}
