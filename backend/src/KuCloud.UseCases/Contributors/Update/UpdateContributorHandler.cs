using KuCloud.Core.Domains.ContributorAggregate;

namespace KuCloud.UseCases.Contributors.Update;

public class UpdateContributorHandler(IRepository<Contributor> repository)
    : ICommandHandler<UpdateContributorCommand, Result<ContributorDto>>
{
    public async Task<Result<ContributorDto>> Handle(UpdateContributorCommand request,
        CancellationToken cancellationToken)
    {
        var existingContributor = await repository.GetByIdAsync(request.ContributorId, cancellationToken);
        if (existingContributor == null) return Result.NotFound();

        existingContributor.UpdateName(request.NewName!);

        await repository.UpdateAsync(existingContributor, cancellationToken);

        return Result.Success(new ContributorDto(existingContributor.Id,
            existingContributor.Name, existingContributor.PhoneNumber?.Number ?? ""));
    }
}