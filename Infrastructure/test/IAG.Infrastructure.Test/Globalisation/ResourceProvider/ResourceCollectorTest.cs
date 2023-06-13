using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.TestHelper.Globalization.ResourceProvider;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.ResourceProvider;

public class ResourceCollectorTest
{
    private readonly ResourceContext _resourceContext;

    public ResourceCollectorTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _resourceContext = ResourceContextBuilder.GetNewContext();
        _resourceContext.Resources.AddRange(new Infrastructure.Globalisation.Model.Resource { Name = "test" });
        _resourceContext.Cultures.AddRange(
            new Culture { Name = "de" },
            new Culture { Name = "fr" });
        _resourceContext.SaveChanges();
    }

    [Fact]
    public void SyncTemplatesToResourcesTest()
    {
        var collector = new ResourceCollector(_resourceContext, null, new MockILogger<ResourceCollector>());
        collector.SyncTemplatesToResources("Test.", new List<IResourceTemplate>
        {
            new ResourceTemplate("testname", "de", "Test-Name")
        });
        // in-memory database is ignored -> no entries
        Assert.Null(_resourceContext.Translations.FirstOrDefault(t => t.Value.Equals("Test-Name")));
    }
}