using KuCloud.Api.Extensions;
using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public class MoveFolderRequest
{
    public const string Route = "/Folder/Move";

    public long Id { get; set; }

    public long ParentId { get; set; }
}

public class MoveFolder(IMediator mediator) : Endpoint<MoveFolderRequest>
{
    public override void Configure()
    {
        Post(MoveFolderRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Move a folder";
            s.ExampleRequest = new MoveFolderRequest { Id = 1, ParentId = 2 };
        });
    }

    public override async Task HandleAsync(MoveFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new MoveFolderCommand(req.Id, req.ParentId, IncludeDeleted: false), ct);

        this.CheckResult(result);

        await SendOkAsync(ct);
    }
}
