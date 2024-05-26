using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

/// <summary>
///     move multiple nodes to a new parent folder
/// </summary>
/// <param name="Ids">list of ids of nodes to be moved</param>
/// <param name="FolderId">new parent folder Id</param>
public sealed record MoveNodeCommand(long[] Ids, long FolderId) : ICommand<Result>;

public sealed class MoveNodeHandler(
    ILogger<MoveNodeHandler> logger,
    IRepository<StorageNode> repos,
    IReadRepository<Folder> folderRepos
) : ICommandHandler<MoveNodeCommand, Result>
{
    public async Task<Result> Handle(MoveNodeCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(MoveNodeCommand)} {request}");

        var nodes = await repos.ListAsync(new MultipleNodesById(request.Ids, includeFolderNesting: true), ct);
        if (nodes.Count == 0)
        {
            logger.LogWarning("Nodes [{Ids}] not found", request.Ids);
            return Result.NotFound("Nodes not found");
        }

        var newParent = await folderRepos.SingleOrDefaultAsync(
            new SingleFolderById(request.FolderId, includeChildren: true),
            ct
        );
        if (newParent is null)
        {
            logger.LogWarning("New parent folder [{Id}] not found", request.FolderId);
            return Result.NotFound("New parent folder not found");
        }

        foreach (var node in nodes)
        {
            node.SetParent(newParent);
        }

        await repos.UpdateRangeAsync(nodes, ct);

        logger.LogInformation(
            "Move nodes [{Ids}] to new parent [{ParentId}]",
            string.Join(", ", request.Ids),
            request.FolderId
        );

        return Result.Success();
    }
}
