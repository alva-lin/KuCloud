using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record DownloadFileRequest
{
    public const string Route = "/storage/download/{Id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public sealed class DownloadFile(IMediator mediator) : Endpoint<DownloadFileRequest>
{
    public override void Configure()
    {
        Get(DownloadFileRequest.Route);
        AllowAnonymous();
        Summary(
            s => {
                s.Summary = "Download a file";
                s.ExampleRequest = new DownloadFileRequest { Id = 1 };
            }
        );
        Description(
            b => b.ClearDefaultProduces()
                .Produces(
                    StatusCodes.Status200OK,
                    responseType: typeof(string),
                    contentType: "application/octet-stream"
                )
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
                .ProducesProblemDetails(StatusCodes.Status500InternalServerError)
        );
    }

    public override async Task HandleAsync(DownloadFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DownloadFileQuery(req.Id), ct);

        this.CheckResult(result);

        var info = result.Value;
        await SendStreamAsync(
            info.Content,
            fileName: info.Name,
            fileLengthBytes: info.Size,
            contentType: info.ContentType,
            lastModified: info.LastModified,
            cancellation: ct
        );
    }
}
