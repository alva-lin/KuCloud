using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record CreateFolderRequest
{
    public const string Route = "/Folder";

    public string Name { get; set; } = null!;

    public long? ParentId { get; set; }
}

public sealed class CreateFolderValidator : Validator<CreateFolderRequest>
{
    public CreateFolderValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.DefaultNodeNameLength);
    }
}

public class CreateFolder(IMediator mediator) : Endpoint<CreateFolderRequest, long>
{
    public override void Configure()
    {
        Post(CreateFolderRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a folder";
            s.ExampleRequest = new CreateFolderRequest { Name = "New Folder", ParentId = null };
        });
    }

    public override async Task HandleAsync(CreateFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateFolderCommand(req.Name, req.ParentId), ct);

        this.CheckResult(result);
        await SendCreatedAtAsync<GetFolder>(
            new { Id = result.Value.Id },
            result.Value.Id,
            cancellation: ct);
    }
}
