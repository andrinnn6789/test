using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;

public class LinkInfoMapper : ObjectMapper<Link, LinkInfo>
{
    protected override LinkInfo MapToDestination(Link source, LinkInfo destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.Url = source.Url;
        destination.Description = source.Description;

        return destination;
    }
}