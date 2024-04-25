using KuCloud.Core.Domains.StorageAggregate;

namespace KuCloud.UseCases.Storages;

// TODO - 修改实现，返回体需要是统一的 dto，和 get nodes 一样

public sealed record GetDeletedNodesQuery(int Page, int PageSize, string? Keyword) : IQuery<Result<List<long>>>;

public sealed class GetDeletedNodesHandler(
    IReadRepository<StorageNode> repos
) : IQueryHandler<GetDeletedNodesQuery, Result<List<long>>>
{
    public async Task<Result<List<long>>> Handle(GetDeletedNodesQuery request, CancellationToken cancellationToken)
    {
        var spec = new DeletedNodesSpec(request.Page, request.PageSize, request.Keyword);
        var data = await repos.ListAsync(spec, cancellationToken);

        return data.Select(e => e.Id).ToList();
    }
}
