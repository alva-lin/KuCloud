using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record RestoreNodeRequest
{
    public const string Route = "/storage/restore";

    public long[] Ids { get; set; } = null!;

    public long FolderId { get; set; }
}

public sealed class RestoreNode(IMediator mediator) : Endpoint<RestoreNodeRequest>
{
    public override void Configure()
    {
        Post(RestoreNodeRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Restore a file";
        });
    }

    public override async Task HandleAsync(RestoreNodeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RestoreNodeCommand(req.Ids, req.FolderId), ct);

        this.CheckResult(result);

        await SendNoContentAsync(ct);
    }
}
