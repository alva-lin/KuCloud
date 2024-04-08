﻿using KuCloud.Core.ContributorAggregate;
using KuCloud.Core.ContributorAggregate.Specifications;

namespace KuCloud.UseCases.Contributors.Get;

/// <summary>
///     Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetContributorHandler(IReadRepository<Contributor> repository)
    : IQueryHandler<GetContributorQuery, Result<ContributorDto>>
{
    public async Task<Result<ContributorDto>> Handle(GetContributorQuery request, CancellationToken cancellationToken)
    {
        var spec = new ContributorByIdSpec(request.ContributorId);
        var entity = await repository.FirstOrDefaultAsync(spec, cancellationToken);
        if (entity == null) return Result.NotFound();

        return new ContributorDto(entity.Id, entity.Name, entity.PhoneNumber?.Number ?? "");
    }
}