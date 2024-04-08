using KuCloud.UseCases.Contributors.Get;
using KuCloud.UseCases.Contributors.Update;

namespace KuCloud.Api.Contributors;

/// <summary>
///     Update an existing Contributor.
/// </summary>
/// <remarks>
///     Update an existing Contributor by providing a fully defined replacement set of values.
///     See:
///     https://stackoverflow.com/questions/60761955/rest-update-best-practice-put-collection-id-without-id-in-body-vs-put-collecti
/// </remarks>
public class Update(IMediator mediator)
    : Endpoint<UpdateContributorRequest, UpdateContributorResponse>
{
    public override void Configure()
    {
        Put(UpdateContributorRequest.Route);
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        UpdateContributorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateContributorCommand(request.Id, request.Name!), cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        var query = new GetContributorQuery(request.ContributorId);

        var queryResult = await mediator.Send(query, cancellationToken);

        if (queryResult.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        if (queryResult.IsSuccess)
        {
            var dto = queryResult.Value;
            Response = new UpdateContributorResponse(new ContributorRecord(dto.Id, dto.Name, dto.PhoneNumber));
        }
    }
}