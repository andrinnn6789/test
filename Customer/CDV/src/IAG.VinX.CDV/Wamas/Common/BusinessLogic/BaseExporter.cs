using System;
using System.Collections.Generic;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public abstract class BaseExporter
{
    private readonly ISybaseConnectionFactory _databaseConnectionFactory;
    private readonly IFtpConnector _ftpConnector;
    private WamasFtpConfig _wamasFtpConfig;

    protected IMessageLogger MessageLogger;
    protected ISybaseConnection DatabaseConnector;
    protected abstract Type[] RecordTypes { get; }

    protected BaseExporter(
        ISybaseConnectionFactory connectionFactory,
        IFtpConnector ftpConnector)
    {
        _databaseConnectionFactory = connectionFactory;
        _ftpConnector = ftpConnector;
    }

    protected void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        _wamasFtpConfig = wamasFtpConfig;
        MessageLogger = messageLogger;
        DatabaseConnector = _databaseConnectionFactory.CreateConnection(connectionString);

        _ftpConnector.SetConfig(_wamasFtpConfig);
    }

    protected void Dispose()
    {
        DatabaseConnector?.Dispose();
    }

    protected void SerializeAndUpload(List<GenericWamasRecord> records, string recordType)
    {
        var data = WamasSerializationHelper.SerializeAsCsv(records, RecordTypes);
        var fileName = $"{recordType}_{DateTime.Now:yyyyMMddHHmmss}.csv";

        _ftpConnector.UploadFile(data, fileName);
    }
}