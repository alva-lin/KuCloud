using KuCloud.Api.Extensions;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.UseCases.Storages.Folders;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record GetFolderRequest
{
    public const string Route = "/Folder/{Id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{Id}", id.ToString());
}

public class GetFolder(IMediator mediator) : Endpoint<GetFolderRequest, StorageNodeDto>
{
    public override void Configure()
    {
        Get(GetFolderRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Get a folder"; });
    }

    public override async Task HandleAsync(GetFolderRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFolderQuery(req.Id), ct);

        this.CheckResult(result);
        await SendAsync(new StorageNodeDto(result.Value), cancellation: ct);
    }
}
