using FastEndpoints;
using KuCloud.UseCases.Contributors.List;
using MediatR;

namespace KuCloud.Api.Contributors;

/// <summary>
///     List all Contributors
/// </summary>
/// <remarks>
///     List all contributors - returns a ContributorListResponse containing the Contributors.
/// </remarks>
public class List(IMediator mediator) : EndpointWithoutRequest<ContributorListResponse>
{
    public override void Configure()
    {
        Get("/Contributors");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListContributorsQuery(null, null), cancellationToken);

        if (result.IsSuccess)
            Response = new ContributorListResponse
            {
                Contributors = result.Value.Select(c => new ContributorRecord(c.Id, c.Name, c.PhoneNumber)).ToList()
            };
    }
}