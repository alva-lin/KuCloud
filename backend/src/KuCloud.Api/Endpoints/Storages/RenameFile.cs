using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record RenameFileRequest
{
    public const string Route = "/File/Rename";

    public long Id { get; set; }

    public string Name { get; set; } = null!;
}

public sealed class RenameFile(IMediator mediator) : Endpoint<RenameFileRequest>
{
    public override void Configure()
    {
        Post(RenameFileRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Rename a file";
            s.ExampleRequest = new RenameFileRequest { Id = 1, Name = "newname.txt" };
        });
    }

    public override async Task HandleAsync(RenameFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RenameFileCommand(req.Id, req.Name), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
