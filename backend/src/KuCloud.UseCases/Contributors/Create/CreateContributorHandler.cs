﻿using KuCloud.Core.ContributorAggregate;

namespace KuCloud.UseCases.Contributors.Create;

public class CreateContributorHandler(IRepository<Contributor> repository)
    : ICommandHandler<CreateContributorCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateContributorCommand request,
        CancellationToken cancellationToken)
    {
        var newContributor = new Contributor(request.Name);
        if (!string.IsNullOrEmpty(request.PhoneNumber)) newContributor.SetPhoneNumber(request.PhoneNumber);
        var createdItem = await repository.AddAsync(newContributor, cancellationToken);

        return createdItem.Id;
    }
}