using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record DeleteNodeRequest
{
    public const string Route = "/storage/delete";

    public long[] Ids { get; set; } = null!;
}

public sealed class DeleteNode(IMediator mediator) : Endpoint<DeleteNodeRequest>
{
    public override void Configure()
    {
        Post(DeleteNodeRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete nodes";
            s.ExampleRequest = new DeleteNodeRequest { Ids = [ 1, 2 ] };
        });
    }

    public override async Task HandleAsync(DeleteNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteNodeCommand(req.Ids), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
