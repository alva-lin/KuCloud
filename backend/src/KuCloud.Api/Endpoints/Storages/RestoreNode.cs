using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record RestoreNodeRequest
{
    public const string Route = "/storage/restore";

    public long[] Ids { get; set; } = null!;

    public long ParentId { get; set; }
}

public sealed class RestoreNode(IMediator mediator) : Endpoint<RestoreNodeRequest>
{
    public override void Configure()
    {
        Post(RestoreNodeRequest.Route);
        AllowAnonymous();
        Summary(
            s => {
                s.Summary = "Restore multiple nodes from trash, and move them to a new folder";
                s.ExampleRequest = new RestoreNodeRequest { Ids = [ 1L, 2L ], ParentId = 3L };
            }
        );
        Description(
            b => b.ClearDefaultProduces()
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(RestoreNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RestoreNodeCommand(req.Ids, req.ParentId), ct);

        this.CheckResult(result);

        await SendNoContentAsync(ct);
    }
}
