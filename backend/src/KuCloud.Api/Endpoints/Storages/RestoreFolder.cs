using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public class RestoreFolderRequest
{
    public const string Route = "/Folder/{Id}/Restore";

    public long Id { get; set; }

    public long ParentId { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public class RestoreFolder(IMediator mediator) : Endpoint<RestoreFolderRequest>
{
    public override void Configure()
    {
        Put(RestoreFolderRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Restore a folder";
            s.ExampleRequest = new RestoreFolderRequest { Id = 1, ParentId = 2 };
        });
    }

    public override async Task HandleAsync(RestoreFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RestoreFolderCommand(req.Id, req.ParentId), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
