using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.Infrastructure.Globalisation.ObjectMapper;

public class CultureMapper: ObjectMapper<Culture, Culture>
{
    protected override Culture MapToDestination(Culture source, Culture destination)
    {
        destination.Name = source.Name;
        return destination;
    }
}