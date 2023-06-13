using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Globalisation.ResourceProvider;

public class ResourceCollectorTest
{
    private readonly ResourceCollector _collector;
    private readonly ResourceContext _resourceContext;
    private readonly MockILogger<ResourceCollector> _logger = new();
    private const string Prefix = "Test.";

    public ResourceCollectorTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>().UseSqlite(connection);
        connection.Open();
        _resourceContext = new ResourceContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        _resourceContext.Database.EnsureCreated();
        _resourceContext.Resources.AddRange(
            new Infrastructure.Globalisation.Model.Resource {Name = "test"},
            new Infrastructure.Globalisation.Model.Resource { Name = Prefix + "test" },
            new Infrastructure.Globalisation.Model.Resource { Name = Prefix + "testname" });
        _resourceContext.Cultures.AddRange(
            new Culture {Name = "de"},
            new Culture {Name = "fr"});
        _resourceContext.SaveChanges();
        _resourceContext.Translations.AddRange(
            new Translation
            {
                Resource = _resourceContext.Resources.First(r => r.Name == "test"),
                Culture = _resourceContext.Cultures.First(r => r.Name == "de"),
                Value = "keep"
            },
            new Translation
            {
                Resource = _resourceContext.Resources.First(r => r.Name == Prefix + "test"),
                Culture = _resourceContext.Cultures.First(r => r.Name == "de"),
                Value = "to remove"
            },
            new Translation
            {
                Resource = _resourceContext.Resources.First(r => r.Name == Prefix + "testname"),
                Culture = _resourceContext.Cultures.First(r => r.Name == "de"),
                Value = "to update"
            },
            new Translation
            {
                Resource = _resourceContext.Resources.First(r => r.Name == Prefix + "testname"),
                Culture = _resourceContext.Cultures.First(r => r.Name == "fr"),
                Value = "Test-Nom"
            }
        );
        _resourceContext.SaveChanges();

        _collector = new ResourceCollector(_resourceContext, null, _logger);
    }

    [Fact]
    public void SyncTemplatesToResourcesTest()
    {
        _collector.SyncTemplatesToResources(Prefix, new List<IResourceTemplate>
        {
            new ResourceTemplate(Prefix + "testname", "de", "Test-Name"),
            new ResourceTemplate(Prefix + "testname", "fr", "Test-Nom"),
            new ResourceTemplate(Prefix + "testname", "it-CH", "Testo nome"),
            new ResourceTemplate(Prefix + "testname", "xx", "Test-xxx"),
            new ResourceTemplate(Prefix + "testnameX", "de", "Test-NameX")
        });
        Assert.NotNull(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("keep")));
        Assert.Null(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("to remove")));
        Assert.Null(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("to update")));
        Assert.NotNull(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("Test-Name")));
        Assert.NotNull(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("Test-Nom")));
        Assert.NotNull(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("Test-NameX")));
        Assert.NotEmpty(_logger.LogEntries);
    }

    [Fact]
    public void SyncTemplatesToResourcesFailTest()
    {
        var collector = new ResourceCollector(_resourceContext, null, _logger);
        collector.CollectAndUpdate();
        Assert.NotEmpty(_logger.LogEntries);
    }
}