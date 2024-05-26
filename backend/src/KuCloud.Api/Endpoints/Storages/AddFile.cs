using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record AddFileRequest
{
    public const string Route = "storage/add-file";

    public long ParentId { get; set; }

    public string Path { get; set; } = null!;

    public string Name { get; set; } = null!;
}

public sealed class AddFileValidator : Validator<AddFileRequest>
{
    public AddFileValidator()
    {
        RuleFor(x => x.ParentId).GreaterThan(0);

        RuleFor(x => x.Path).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(DataSchemaConstants.DefaultNodeNameLength);
    }
}

public sealed class AddFile(IMediator mediator) : Endpoint<AddFileRequest, long>
{
    public override void Configure()
    {
        Post(AddFileRequest.Route);
        AllowAnonymous();
        Summary(
            s => {
                s.Summary = "Add a file";
                s.ExampleRequest = new AddFileRequest { ParentId = 1, Path = "/path/to/file", Name = "file.txt" };
            }
        );
        Description(
            b => b.ClearDefaultProduces()
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
        );
    }

    public override async Task HandleAsync(AddFileRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddFileCommand(req.ParentId, req.Path, req.Name), ct);

        this.CheckResult(result);

        await SendOkAsync(result.Value, ct);
    }
}
