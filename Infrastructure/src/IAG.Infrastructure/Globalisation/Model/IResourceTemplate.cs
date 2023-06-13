// 
// 2020 09 17 15:07

namespace IAG.Infrastructure.Globalisation.Model;

public interface IResourceTemplate
{
    string Name { get; }
    string Language { get; }
    string Translation { get; }
}