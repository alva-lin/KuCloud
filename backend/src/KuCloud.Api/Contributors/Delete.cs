using Ardalis.Result;
using FastEndpoints;
using KuCloud.UseCases.Contributors.Delete;
using MediatR;

namespace KuCloud.Api.Contributors;

/// <summary>
///     Delete a Contributor.
/// </summary>
/// <remarks>
///     Delete a Contributor by providing a valid integer id.
/// </remarks>
public class Delete(IMediator mediator)
    : Endpoint<DeleteContributorRequest>
{
    public override void Configure()
    {
        Delete(DeleteContributorRequest.Route);
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        DeleteContributorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContributorCommand(request.ContributorId);

        var result = await mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        if (result.IsSuccess) await SendNoContentAsync(cancellationToken);
        ;
        // TODO: Handle other issues as needed
    }
}