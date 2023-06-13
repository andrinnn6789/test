using System;

using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.Configuration;

public class ConfigCommonStoreDbContextTest
{
    private readonly ConfigCommonDbContext _context;

    public ConfigCommonStoreDbContextTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigCommonDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new ConfigCommonDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
    }

    [Fact]
    public void TestContext()
    {
        var id = Guid.NewGuid();
        var data = "TestData";

        _context.ConfigCommonEntries.Add(new ConfigCommon {Id = id, Data = data});
        _context.SaveChanges();

        var entry = Assert.Single(_context.ConfigCommonEntries);
        Assert.NotNull(entry);
        Assert.Equal(id, entry.Id);
        Assert.Equal(data, entry.Data);
    }
}