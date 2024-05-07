using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record MoveNodeRequest
{
    public const string Route = "/storage/move";

    public long[] Ids { get; set; } = null!;

    public long FolderId { get; set; }
}

public sealed class MoveNode(IMediator mediator) : Endpoint<MoveNodeRequest>
{
    public override void Configure()
    {
        Post(MoveNodeRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Move nodes to new folder"; });
    }

    public override async Task HandleAsync(MoveNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new MoveNodeCommand(req.Ids, req.FolderId), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
