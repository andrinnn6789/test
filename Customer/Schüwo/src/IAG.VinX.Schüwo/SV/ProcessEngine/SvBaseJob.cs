using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.SV.BusinessLogic;
using IAG.VinX.Schüwo.SV.Dto.Interface;

namespace IAG.VinX.Schüwo.SV.ProcessEngine;

public abstract class SvBaseJob<TConfig, TParam, TResult> : JobBase<TConfig, TParam, TResult>
    where TConfig : class, ISvBaseJobConfig, IJobConfig, new()
    where TParam : class, IJobParameter, new()
    where TResult : SvBaseJobResult, new()
{
    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    private DataFormatter _formatter;
    private FtpConnector _connector;
    protected DataExtractor Extractor;

    protected SvBaseJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinXConnectionString);
        Extractor = new DataExtractor(sybaseConnection, this, Result.ResultCounts);
        try
        {
            _connector = new FtpConnector(Config.FtpEndpointConfig, Config.FtpPathConfig);

            Sync();
        }
        finally
        {
            _connector.Dispose();
        }

        Result.Result = Result.ResultCounts.WarningCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;

        base.ExecuteJob();
    }

    protected abstract void Sync();

    protected int FormatAndUploadData<T>(IReadOnlyCollection<T> data, string dataName)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8);

        var dataTypeInterfaces = data.GetType().GetGenericArguments().First().GetInterfaces();
        if (dataTypeInterfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOrder<>)))
        {
            Formatter.FormatOrderListToCsv(writer, data as IEnumerable<IOrder<IOrderPos>>, dataName);
        }
        else
        {
            Formatter.FormatDataListToCsv(writer, data, dataName);
        }
        DataFormatter.WriteLineCountFooter(stream, writer);
        _connector.UploadFile(stream, dataName + Config.FtpPathConfig.Extension);

        Result.ResultCounts.SuccessCount++;
        return data.Count;
    }

    private DataFormatter Formatter => _formatter ??= new DataFormatter();
}