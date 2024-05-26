using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record RenameNodeRequest
{
    public const string Route = "/storage/rename";

    public long Id { get; set; }

    public string Name { get; set; } = null!;
}

public sealed class RenameNode(IMediator mediator) : Endpoint<RenameNodeRequest>
{
    public override void Configure()
    {
        Post(RenameNodeRequest.Route);
        AllowAnonymous();
        Summary(
            s => {
                s.Summary = "Rename a file";
                s.ExampleRequest = new RenameNodeRequest { Id = 1, Name = "newname.txt" };
            }
        );
        Description(
            b => b.ClearDefaultProduces()
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
                .ProducesProblemDetails(StatusCodes.Status409Conflict)
        );
    }

    public override async Task HandleAsync(RenameNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RenameNodeCommand(req.Id, req.Name), ct);

        this.CheckResult(result);

        await SendNoContentAsync(ct);
    }
}
