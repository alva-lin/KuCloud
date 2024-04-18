using KuCloud.Core.Domains.ContributorAggregate;
using KuCloud.Core.Domains.ContributorAggregate.Events;

namespace KuCloud.UseCases.Contributors.Delete;

public class DeleteContributorHandler(IRepository<Contributor> repository, IMediator mediator)
    : ICommandHandler<DeleteContributorCommand, Result>
{
    public async Task<Result> Handle(DeleteContributorCommand request, CancellationToken cancellationToken)
    {
        var aggregateToDelete = await repository.GetByIdAsync(request.ContributorId, cancellationToken);
        if (aggregateToDelete == null) return Result.NotFound();

        await repository.DeleteAsync(aggregateToDelete, cancellationToken);
        var domainEvent = new ContributorDeletedEvent(request.ContributorId);
        await mediator.Publish(domainEvent, cancellationToken);
        return Result.Success();
    }
}
