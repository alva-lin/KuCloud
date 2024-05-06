namespace KuCloud.Api.Endpoints;

public class Status(AppStatusMonitor monitor) : EndpointWithoutRequest<AppStatusInfo>
{
    public override void Configure()
    {
        Get("/system/status");
        AllowAnonymous();
        Summary(s => { s.Summary = "Get System Status"; });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync(monitor.GetStatus(), ct);
    }
}
