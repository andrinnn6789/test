using System;
using System.Linq;

using IAG.Infrastructure.Globalisation.Admin;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Admin;

public class ResourceAdminTest
{
    private ResourceContext _context;
    private readonly DbContextOptionsBuilder<ResourceContext> _optionsBuilder;

    public ResourceAdminTest()
    {
        _optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new ResourceContext(_optionsBuilder.Options, new ExplicitUserContext("test", null));
        _context.Cultures.AddRange(
            new Culture {Name = "de"},
            new Culture {Name = "xx"});
        _context.Resources.AddRange(
            new Infrastructure.Globalisation.Model.Resource {Name = "r1"},
            new Infrastructure.Globalisation.Model.Resource {Name = "r2"},
            new Infrastructure.Globalisation.Model.Resource {Name = "x1"},
            new Infrastructure.Globalisation.Model.Resource {Name = "x2"});
        _context.SaveChanges();
    }

    [Fact]
    public void SyncSystemsTest()
    {
        var transSync = new TranslationSync();
        transSync.Cultures.AddRange(_context.Cultures.Where(c => c.Name.Equals("de")).AsNoTracking());
        transSync.Resources.AddRange(_context.Resources.Where(r => r.Name.StartsWith("r")).AsNoTracking());
        transSync.Resources.Add(new Infrastructure.Globalisation.Model.Resource { Name = "r3" });
        transSync.Translations.AddRange(new[]
            {
                new Translation
                {
                    ResourceId = transSync.Resources[0].Id,
                    CultureId = transSync.Cultures[0].Id,
                    Value = "trans1"
                },
                new Translation
                {
                    ResourceId = transSync.Resources[0].Id,
                    CultureId = transSync.Cultures[0].Id,
                    Value = "trans2"
                },
                new Translation
                {
                    ResourceId = transSync.Resources.First(r => r.Name.Equals("r3")).Id,
                    CultureId = transSync.Cultures[0].Id,
                    Value = "trans3"
                }
            }
        );

        _context = new ResourceContext(_optionsBuilder.Options, new ExplicitUserContext("test", null));
        var admin = new ResourceAdmin(_context);
        admin.SyncSystems(transSync);
        Assert.Equal(3, _context.Translations.Count());
        Assert.Equal(2, _context.Cultures.Count());
        Assert.Equal(5, _context.Resources.Count());
    }

    [Fact]
    public void GetFlatTest()
    {
        _context.Translations.AddRange(
            new Translation
            {
                ResourceId = _context.Resources.First().Id,
                CultureId = _context.Cultures.First(c => c.Name.Equals("de")).Id,
                Value = "trans1"
            },
            new Translation
            {
                ResourceId = _context.Resources.First().Id,
                CultureId = _context.Cultures.First(c => c.Name.Equals("xx")).Id,
                Value = "trans2"
            }
        );
        _context.SaveChanges();

        var admin = new ResourceAdmin(_context);
        var transFlat = admin.GetFlat(null);
        Assert.Equal(2, transFlat.Count);
        transFlat = admin.GetFlat("de");
        Assert.Single(transFlat);
        Assert.Equal("r1", transFlat[0].Resource);
        Assert.Equal("de", transFlat[0].Culture);
        Assert.Equal("trans1", transFlat[0].Value);
        transFlat = admin.GetFlat("aa");
        Assert.Empty(transFlat);
    }
}