using Microsoft.OData.ModelBuilder;

namespace IAG.Infrastructure.Rest;

public interface IEdmModelBuilder
{
    void AddModels(ODataConventionModelBuilder builder);
}