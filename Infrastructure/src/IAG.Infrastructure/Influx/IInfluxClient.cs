using System.Collections.Generic;
using System.Threading.Tasks;

using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace IAG.Infrastructure.Influx;

public interface IInfluxClient
{
    Task SendDataPointAsync(string bucket, PointData dataPoint);
    Task<List<FluxTable>> QueryAsync(string fluxQuery, string org);
}