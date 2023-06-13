using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.ControlCenter.Test.Distribution.BusinessLayer;

public class LinkAdminHandlerTest : IDisposable
{
    private readonly DistributionDbContext _context;
    private readonly LinkAdminHandler _handler;

    public LinkAdminHandlerTest()
    {
        var options = new DbContextOptionsBuilder<DistributionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new DistributionDbContext(options, new ExplicitUserContext("test", null));

        _handler = new LinkAdminHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SyncLinksTest()
    {
        var oldLink = new Link() { Name = "OldLink", Url = "www.old-link.net" };
        _context.Links.Add(oldLink);
        await _context.SaveChangesAsync();

        var newLinks = new List<LinkRegistration>()
        {
            new() {Name = "NewLink1", Url = "www.new-link.one", Description = "Description One"},
            new() {Name = "NewLink2", Url = "www.new-link.two", Description = "Description Two"}
        };

        var links = await _handler.SyncLinksAsync(newLinks);

        Assert.Equal(2, _context.Links.Count());
        Assert.Equal(2, links.Count());
        Assert.All(newLinks, link => Assert.Contains(links, resLink => resLink.Name == link.Name && resLink.Url == link.Url && link.Description == resLink.Description));
        Assert.DoesNotContain(_context.Links, dbLink => dbLink.Name == oldLink.Name);
        Assert.All(newLinks, link => Assert.Contains(_context.Links, dbLink => dbLink.Name == link.Name && dbLink.Url == link.Url && link.Description == dbLink.Description));
    }
}