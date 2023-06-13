using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public class LinkAdminHandler : ILinkAdminHandler
{
    private readonly DistributionDbContext _context;

    public LinkAdminHandler(DistributionDbContext context)
    {
        _context = context;
    }

    public async Task<List<LinkInfo>> SyncLinksAsync(List<LinkRegistration> links)
    {
        _context.Links.RemoveRange(_context.Links);
        _context.Links.AddRange(
            links.Select(link => new Link()
            {
                Name = link.Name,
                Url = link.Url,
                Description = link.Description
            })
        );

        await _context.SaveChangesAsync();

        var mapper = new LinkInfoMapper();
        return await _context.Links
            .AsNoTracking()
            .Select(l => mapper.NewDestination(l))
            .ToListAsync();
    }
}