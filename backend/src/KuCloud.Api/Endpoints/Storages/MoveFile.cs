using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record MoveFileRequest
{
    public const string Route = "/File/Move";

    public long[] Ids { get; set; } = null!;

    public long FolderId { get; set; }
}

public sealed class MoveFile(IMediator mediator) : Endpoint<MoveFileRequest>
{
    public override void Configure()
    {
        Post(MoveFileRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Move a file"; });
    }

    public override async Task HandleAsync(MoveFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new MoveFileCommand(req.Ids, req.FolderId), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
