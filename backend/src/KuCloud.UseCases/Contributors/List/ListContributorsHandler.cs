using KuCloud.Core.ContributorAggregate;

namespace KuCloud.UseCases.Contributors.List;

public class ListContributorsHandler(IReadRepository<Contributor> repository)
    : IQueryHandler<ListContributorsQuery, Result<IEnumerable<ContributorDto>>>
{
    public async Task<Result<IEnumerable<ContributorDto>>> Handle(ListContributorsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await repository.ListAsync(cancellationToken);

        var result = data.Select(e => new ContributorDto(e.Id, e.Name, null));

        return Result.Success(result);
    }
}
