using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public abstract class BaseImporter
{
    private readonly ISybaseConnectionFactory _databaseConnectionFactory;
    private readonly IFtpConnector _ftpConnector;
    private WamasFtpConfig _wamasFtpConfig;

    protected IMessageLogger MessageLogger;
    protected ISybaseConnection DatabaseConnector;
    protected abstract Tuple<Type, string>[] RecordTypeMappings { get; }

    protected BaseImporter(
        ISybaseConnectionFactory databaseConnectionFactory,
        IFtpConnector ftpConnector)
    {
        _databaseConnectionFactory = databaseConnectionFactory;
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

    protected List<string> SearchFilesToImport(string recordType)
    {
        return _ftpConnector.GetFiles($"{recordType}", "csv");
    }

    protected List<GenericWamasRecord> DownloadAndDeserialize(string file)
    {
        var data = Download(file);

        return RecordTypeMappings.Length > 1
            ? WamasSerializationHelper.DeserializeFromCsv(data, RecordTypeMappings)
            : WamasSerializationHelper.DeserializeFromCsv(data, RecordTypeMappings.First());
    }

    protected void ArchiveFile(string file, bool wasSuccessful)
    {
        _ftpConnector.ArchiveFile(file, wasSuccessful);
    }

    private byte[] Download(string file)
    {
        return _ftpConnector.DownloadFile(file);
    }
}