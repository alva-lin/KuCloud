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

        var spec = new MultipleNodesById(request.Ids, includeFolderNesting: true);
        var nodes = await repos.ListAsync(spec, ct);
        if (nodes.Count > 0)
        {
            foreach (var node in nodes)
            {
                node.SetParent(null);
                node.AuditInfo.SetDeleteInfo();
            }

            await repos.UpdateRangeAsync(nodes, ct);

            logger.LogInformation("Delete nodes [{Ids}]", string.Join(", ", request.Ids));
        }

        return Result.Success();
    }
}
