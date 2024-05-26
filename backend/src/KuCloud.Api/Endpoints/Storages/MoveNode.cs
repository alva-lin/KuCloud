using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record MoveNodeRequest
{
    public const string Route = "/storage/move";

    public long[] Ids { get; set; } = null!;

    public long ParentId { get; set; }
}

public sealed class MoveNode(IMediator mediator) : Endpoint<MoveNodeRequest>
{
    public override void Configure()
    {
        Post(MoveNodeRequest.Route);
        AllowAnonymous();
        Summary(
            s => {
                s.Summary = "Move nodes to new folder";
                s.ExampleRequest = new MoveNodeRequest { Ids = [ 1L, 2L ], ParentId = 3L };
            }
        );
        Description(
            b => b.ClearDefaultProduces()
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(MoveNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new MoveNodeCommand(req.Ids, req.ParentId), ct);

        this.CheckResult(result);

        await SendNoContentAsync(ct);
    }
}
