using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record UploadFileRequest
{
    public const string Route = "/storage/upload";

    public IFormFile File { get; set; } = null!;
}

public sealed class UploadFile(IMediator mediator) : Endpoint<UploadFileRequest, string>
{
    public override void Configure()
    {
        Post(UploadFileRequest.Route);
        AllowAnonymous();
        AllowFileUploads();
        Summary(s => { s.Summary = "Upload a file"; });
    }

    public override async Task HandleAsync(UploadFileRequest req, CancellationToken ct)
    {
        var stream = req.File.OpenReadStream();
        var result = await mediator.Send(new UploadFileCommand(stream), ct);

        this.CheckResult(result);

        await SendOkAsync(result.Value, ct);
    }
}
