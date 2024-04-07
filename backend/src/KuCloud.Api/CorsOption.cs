namespace KuCloud.Api;

public class CorsOption
{
    public string[] AllowHeaders { get; set; } = [];

    public string[] AllowExposedHeaders { get; set; } = [];

    public string[] AllowMethods { get; set; } = [];

    public string[] AllowOrigins { get; set; } = [];

    public bool? AllowAnyHeader { get; set; }

    public bool? AllowAnyMethod { get; set; }

    public bool? AllowAnyOrigin { get; set; }
}
