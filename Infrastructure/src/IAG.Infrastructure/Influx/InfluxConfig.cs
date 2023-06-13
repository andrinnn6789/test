namespace IAG.Infrastructure.Influx;

public class InfluxConfig
{
    public string Url { get; set; }
    public string Token { get; set; }
    public string Org { get; set; } = "iag";
}