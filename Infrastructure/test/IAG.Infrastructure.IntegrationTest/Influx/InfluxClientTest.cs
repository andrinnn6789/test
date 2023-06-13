using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Influx;

using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Exceptions;
using InfluxDB.Client.Writes;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Influx;

public class InfluxClientTest
{
    private const string Bucket = "influxtest";

    [Fact]
    public async Task SendTest()
    {
        var influxClient = CreateInfluxClient();
        var dataPoint = PointData.Measurement("Integration Test")
            .Field("Is it working?", "true")
            .Tag("TestTag", "TestTag")
            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);
        await influxClient.SendDataPointAsync(Bucket, dataPoint);
        var query = "from(bucket:\"influxtest\") |> range(start: -1s, stop: 2s)";
        var results = await influxClient.QueryAsync(query, "iag");
        
        Assert.NotEmpty(results);
        var result = results.First();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Columns);
        Assert.Contains(result.Columns, c => c.Label == "TestTag");
    }

    [Fact]
    public async Task SendExceptionTest()
    {
        var influxClient = CreateInfluxClient("invalid");

        var dataPoint = PointData.Measurement("Integration Test")
            .Field("Will not work", "true")
            .Tag("TestTag", "TestTag")
            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

        await Assert.ThrowsAsync<UnauthorizedException>(() => influxClient.SendDataPointAsync(Bucket, dataPoint));
    }

    private InfluxClient CreateInfluxClient(string token = null)
    {
        return new InfluxClient(
            new InfluxConfig
            {
                Url = "https://monitor.i-ag.ch/influxdb/",
                Token = token ?? "_WkQ8IWlthmQTBLtWaij86DGK9Wg7HdFjxBPUylJJiaZAsPmJwK8rusaaSDLd4suZKjAEHzSdIfJZR-14uTU5A==",
                Org = "iag"
            }
        );
    }
}