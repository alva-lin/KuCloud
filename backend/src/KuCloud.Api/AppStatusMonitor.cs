namespace KuCloud.Api;

public class AppStatusMonitor
{
    public readonly DateTime StartTime = DateTime.UtcNow;

    public AppStatusInfo GetStatus()
    {
        var currentTime = DateTime.UtcNow;
        var uptime = currentTime - StartTime;
        return new AppStatusInfo
        {
            StartTime = StartTime,
            CurrentTime = currentTime,
            Uptime = uptime
        };
    }
}

public class AppStatusInfo
{
    public DateTime StartTime { get; set; }

    public DateTime CurrentTime { get; set; }

    public TimeSpan Uptime { get; set; }
}
