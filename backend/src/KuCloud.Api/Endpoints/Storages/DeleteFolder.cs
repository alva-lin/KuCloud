using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record DeleteFolderRequest
{
    public const string Route = "/Folder/{Id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public class DeleteFolder(IMediator mediator) : Endpoint<DeleteFolderRequest>
{
    public override void Configure()
    {
        Delete(DeleteFolderRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Delete a folder";
            s.ExampleRequest = new DeleteFolderRequest { Id = 1 };
        });
    }

    public override async Task HandleAsync(DeleteFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFolderCommand(req.Id), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
