using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record RestoreFileRequest
{
    public const string Route = "/File/Restore";

    public long[] Ids { get; set; } = null!;

    public long FolderId { get; set; }
}

public sealed class RestoreFile(IMediator mediator) : Endpoint<RestoreFileRequest>
{
    public override void Configure()
    {
        Post(RestoreFileRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Restore a file";
        });
    }

    public override async Task HandleAsync(RestoreFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RestoreFileCommand(req.Ids, req.FolderId), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
