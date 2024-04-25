using KuCloud.UseCases.Storages;

namespace KuCloud.Api.Endpoints.Storages;

public sealed record GetDeletedNodesRequest
{
    public const string Route = "/DeletedNodes";

    public int Page { get; set; }

    public int PageSize { get; set; }

    public string? Keyword { get; set; }
}

public sealed class GetDeletedNodesValidator : Validator<GetDeletedNodesRequest>
{
    public GetDeletedNodesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}

public sealed class GetDeletedNodes(IMediator mediator) : Endpoint<GetDeletedNodesRequest, List<long>>
{
    public override void Configure()
    {
        Get(GetDeletedNodesRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Get deleted nodes"; });
    }

    public override async Task HandleAsync(GetDeletedNodesRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDeletedNodesQuery(req.Page, req.PageSize, req.Keyword), ct);

        this.CheckResult(result);
        await SendAsync(result.Value, cancellation: ct);
    }
}
