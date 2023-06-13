using System.IO;
using System.Linq;

using IAG.Common.ArchiveProvider;
using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Event;

using Xunit;

using FileHandler = IAG.PerformX.CampusSursee.Sybase.FileHandler;

namespace IAG.PerformX.CampusSursee.IntegrationTest.Sybase;

public class FileHandlerTest
{
    private readonly FileHandler _fileHandler;
    private readonly ISybaseConnection _connection;

    public FileHandlerTest()
    {
        _connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(),
            Startup.BuildConfig(),
            null).CreateConnection();

        var fileArchiveFactory = new ArchiveProviderFactory(new PluginLoader());

        _fileHandler = new FileHandler(_connection, fileArchiveFactory, new MockILogger<FileHandlerTest>());
    }

    [Fact]
    public void GetDocFromDocumentTest()
    {
        var doc = _connection.GetQueryable<Document>().Take(1).ToList().First();
        var (docName, docData) = _fileHandler.GetDocumentData(doc.Id);
        Assert.NotEmpty(docName);
        Assert.NotEmpty(docData);
    }

    [Fact]
    public void GetDocFromDocumentNotFoundTest()
    {
        Assert.Throws<FileNotFoundException>(() => _fileHandler.GetDocumentData(1));
        Assert.Throws<FileNotFoundException>(() => _fileHandler.GetDocumentData(2_000_000_000)); 
        Assert.Throws<FileNotFoundException>(() => _fileHandler.GetDocumentData(1_099_000_000)); 
        var additional = _connection.GetQueryable<Additional>().Where(a => a.AtlasDocumentId > 0).Take(1).ToList().First();
        Assert.NotNull(additional.DocumentId);
        Assert.Throws<FileNotFoundException>(() => _fileHandler.GetDocumentData(additional.DocumentId.Value));
    }

    [Fact]
    public void GetDocFromAdditionalTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var additional = _connection.GetQueryable<Additional>().Where(a => a.AtlasDocumentId > 0).Take(1).ToList().First();
            _connection
                .CreateCommand(
                    @"UPDATE VertragDefZusatz SET VertragDefZusatz_Datei = ? WHERE VertragDefZusatz_Id = ?",
                    Path.Combine("Dto", "Test.pdf"), additional.Id)
                .ExecuteNonQuery();
            Assert.NotNull(additional.DocumentId);
            var (docName, docData) = _fileHandler.GetDocumentData(additional.DocumentId.Value);
            Assert.NotEmpty(docName);
            Assert.NotEmpty(docData);
        });
    }
}