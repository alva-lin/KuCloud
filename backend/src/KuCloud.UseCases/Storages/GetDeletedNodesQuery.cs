using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

public sealed record GetDeletedNodesQuery(int Page, int PageSize, string? Keyword)
    : IQuery<Result<PaginatedList<StorageNodeDto>>>;

public sealed class GetDeletedNodesHandler(
    IReadRepository<StorageNode> repos
) : IQueryHandler<GetDeletedNodesQuery, Result<PaginatedList<StorageNodeDto>>>
{
    public async Task<Result<PaginatedList<StorageNodeDto>>> Handle(GetDeletedNodesQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new DeletedNodesSpec(request.Page, request.PageSize, request.Keyword);

        var count = await repos.CountAsync(spec, cancellationToken);
        var data = await repos.ListAsync(spec, cancellationToken);

        return new PaginatedList<StorageNodeDto>(data.Select(StorageNodeDto.Create), count);
    }
}
