using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record GetFolderRequest
{
    public const string Route = "/storage/{Id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public sealed class GetFolder(IMediator mediator) : Endpoint<GetFolderRequest, FolderDto>
{
    public override void Configure()
    {
        Get(GetFolderRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Get a folder"; });
    }

    public override async Task HandleAsync(GetFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFolderQuery(req.Id), ct);

        this.CheckResult(result);
        await SendAsync(result.Value, cancellation: ct);
    }
}
