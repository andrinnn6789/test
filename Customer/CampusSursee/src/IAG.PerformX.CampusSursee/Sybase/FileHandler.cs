using System;
using System.IO;
using System.Linq;

using IAG.Common.ArchiveProvider;
using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Logging;
using IAG.PerformX.CampusSursee.Dto;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Event;

using Microsoft.Extensions.Logging;

namespace IAG.PerformX.CampusSursee.Sybase;

public class FileHandler
{
    private readonly ISybaseConnection _connection;
    private readonly IArchiveProviderFactory _archiveProviderFactory;
    private readonly ILogger _logger;
    private int _atlasId;
    private int _docId;

    public FileHandler(ISybaseConnection sybaseConnection, IArchiveProviderFactory archiveProviderFactory, ILogger logger)
    {
        _connection = sybaseConnection;
        _archiveProviderFactory = archiveProviderFactory;
        _logger = logger;
    }

    public (string docName, byte[] docData) GetDocumentData(int docId)
    {
        _docId = docId;
        return GetDocType() switch
        {
            DocumentOffsetEnum.Additional => GetDocumentFromAdditional(),
            DocumentOffsetEnum.ArchiveLink => GetDocumentFromArchiveLink(),
            _ => throw new FileNotFoundException(_docId.ToString())
        };
    }

    private DocumentOffsetEnum GetDocType()
    {
        if (_docId < (int)DocumentOffsetEnum.Additional)
            throw new FileNotFoundException(_docId.ToString());
        var docType = _docId < (int) DocumentOffsetEnum.ArchiveLink ? DocumentOffsetEnum.Additional : DocumentOffsetEnum.ArchiveLink;
        _atlasId = _docId - (int) docType;
        return docType;
    }

    private (string docName, byte[] docData) GetDocumentFromAdditional()
    {
        var additional = _connection.GetQueryable<Additional>().Where(a => a.Id == _atlasId).Take(1).ToList().FirstOrDefault();
        if (additional == null)
            throw new FileNotFoundException(_docId.ToString());
        return ExecutewithLogger(() => (additional.FileName, File.ReadAllBytes(additional.FileName)));
    }

    private (string docName, byte[] docData) GetDocumentFromArchiveLink()
    {
        var document = _connection.GetQueryable<Document>().Where(a => a.AtlasId == _atlasId).ToList().FirstOrDefault();
        if (document == null)
            throw new FileNotFoundException(_docId.ToString());
        return ExecutewithLogger(() => 
        {
            var file = _archiveProviderFactory.GetFileContent(_connection, document.ArchiveLinkUri);

            return (document.FileName, file.Content);
        });
    }

    private (string docName, byte[] docData) ExecutewithLogger(Func<(string, byte[])> f)
    {
        try
        {
            return f();
        }
        catch (Exception e)
        {
            new ErrorLogger().LogException(_logger, e);
            throw new FileNotFoundException(_docId.ToString());
        }
    }
}