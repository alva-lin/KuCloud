namespace KuCloud.Api.Endpoints.System;

public sealed record ReturnsByCodeRequest
{
    public int Code { get; set; }
}

public sealed class ReturnsByCode : Endpoint<ReturnsByCodeRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("/system/returns-by-code/{Code}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get returns by code";
        });
    }

    public override async Task HandleAsync(ReturnsByCodeRequest req, CancellationToken ct)
    {
        await SendAsync(new EmptyResponse(), req.Code, ct);
    }
}
