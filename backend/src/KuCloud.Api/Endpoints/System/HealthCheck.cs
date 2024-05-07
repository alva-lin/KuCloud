namespace KuCloud.Api.Endpoints;

public sealed class HealthCheck : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Get("/system/health");
        AllowAnonymous();
        Summary(s => { s.Summary = "Health check"; });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync(ct);
    }
}
