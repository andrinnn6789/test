using IAG.Infrastructure.ObjectMapper;

namespace IAG.Infrastructure.Globalisation.ObjectMapper;

public class ResourceMapper: ObjectMapper<Model.Resource, Model.Resource>
{
    protected override Model.Resource MapToDestination(Model.Resource source, Model.Resource destination)
    {
        destination.Name = source.Name;
        return destination;
    }
}