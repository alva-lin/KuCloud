using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record RenameNodeCommand(long Id, string NewName) : ICommand<Result>;

public sealed class RenameNodeHandler(
    ILogger<RenameNodeHandler> logger,
    IRepository<StorageNode> repos
) : ICommandHandler<RenameNodeCommand, Result>
{
    public async Task<Result> Handle(RenameNodeCommand request, CancellationToken ct)
    {
        using var _ = logger.BeginScope($"Handle {nameof(RenameNodeCommand)} {request}");

        var specs = new SingleNodeById(request.Id, includeParents: true);
        var node = await repos.SingleOrDefaultAsync(specs, ct);

        if (node is null)
        {
            logger.LogWarning("Node [{Id}] not found", request.Id);
            return Result.NotFound("Node not found");
        }

        var oldName = node.Name;
        var newName = request.NewName.Trim();
        if (oldName == newName)
        {
            logger.LogWarning("New name is same as old name");
            return Result.Conflict("New name is same as old name");
        }

        node.SetName(newName);
        await repos.UpdateAsync(node, ct);

        logger.LogInformation("Rename node [{Id}] {OldName} -> {NewName}", node.Id, oldName, newName);

        return Result.Success();
    }
}
