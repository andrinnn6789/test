using System.Composition;

using IAG.Infrastructure.Rest;

using JetBrains.Annotations;

using Microsoft.OData.ModelBuilder;

namespace IAG.Infrastructure.IntegrationTest.Controller;

[Export(typeof(IEdmModelBuilder))]
public class KeyEdm : IEdmModelBuilder
{
    [UsedImplicitly]
    public void AddModels(ODataConventionModelBuilder builder)
    {
        builder.EntitySet<NumKey>("NumKeysOData");
    }
}