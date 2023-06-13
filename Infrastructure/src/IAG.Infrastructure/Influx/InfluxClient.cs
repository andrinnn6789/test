#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;

using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace IAG.Infrastructure.Influx;

public class InfluxClient : IInfluxClient
{
    private readonly InfluxDBClient _influxDbClient;
    private readonly string _influxDbOrg;

    public InfluxClient(InfluxConfig influxConfig)
    {
        _influxDbClient = InfluxDBClientFactory.Create(influxConfig.Url, influxConfig.Token.ToCharArray());
        _influxDbOrg = influxConfig.Org;
    }

    public async Task SendDataPointAsync(string bucket, PointData dataPoint)
    {
        var writeApi = _influxDbClient.GetWriteApiAsync();
        await writeApi.WritePointAsync(dataPoint, bucket, _influxDbOrg);
    }

    public Task<List<FluxTable>> QueryAsync(string fluxQuery, string org)
    {
        var result = _influxDbClient.GetQueryApi().QueryAsync(fluxQuery, org);
        return result;
    }
}