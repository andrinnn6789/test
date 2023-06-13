using System.IO;
using System.Reflection;

using InfluxDB.Client.Writes;

namespace IAG.Infrastructure.Influx;

public class DatapointBuilder
{
    public PointData BuildPoint(string measurement, string customerName)
    {
        var dirName = new DirectoryInfo(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\")).Name;
        return PointData.Measurement(measurement)
            .Tag("customerName", customerName)
            .Tag("instanceName", dirName);
    }
}