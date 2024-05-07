using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed class UploadFileRequest
{
    public const string Route = "/storage/upload";

    public IFormFile File { get; set; } = null!;
}

public sealed record UploadFileResponse(string Path);

public class UploadFile(IMediator mediator) :Endpoint<UploadFileRequest, UploadFileResponse>
{
    public override void Configure()
    {
        Post(UploadFileRequest.Route);
        AllowAnonymous();
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload a file";
        });
    }

    public override async Task HandleAsync(UploadFileRequest req, CancellationToken ct)
    {
        var stream = req.File.OpenReadStream();
        var result = await mediator.Send(new UploadFileCommand(stream), ct);

        this.CheckResult(result);

        await SendOkAsync(new UploadFileResponse(result.Value),ct);
    }
}
