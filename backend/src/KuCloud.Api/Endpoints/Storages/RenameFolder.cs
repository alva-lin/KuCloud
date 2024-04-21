using KuCloud.Api.Extensions;
using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public class RenameFolderRequest
{
    public const string Route = "/Folder/{Id}/Rename";

    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public class RenameFolderValidator : Validator<RenameFolderRequest>
{
    public RenameFolderValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.DefaultNodeNameLength);
    }
}

public class RenameFolder(IMediator mediator) : Endpoint<RenameFolderRequest>
{
    public override void Configure()
    {
        Put(RenameFolderRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Rename a folder";
            s.ExampleRequest = new RenameFolderRequest { Name = "New Folder" };
        });
    }

    public override async Task HandleAsync(RenameFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new RenameFolderCommand(req.Id, req.Name), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
