using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record DeleteFileRequest
{
    public const string Route = "/File/{Id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public sealed class DeleteFile(IMediator mediator) : Endpoint<DeleteFileRequest>
{
    public override void Configure()
    {
        Delete(DeleteFileRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a file";
            s.ExampleRequest = new DeleteFileRequest { Id = 1 };
        });
    }

    public override async Task HandleAsync(DeleteFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFileCommand(req.Id), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
