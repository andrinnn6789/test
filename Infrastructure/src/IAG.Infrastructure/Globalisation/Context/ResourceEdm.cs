using System.Composition;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Rest;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

namespace IAG.Infrastructure.Globalisation.Context;

[Export(typeof(IEdmModelBuilder))]
public class ResourceEdm : DbContext, IEdmModelBuilder
{
    [UsedImplicitly]
    public void AddModels(ODataConventionModelBuilder builder)
    {
        builder.EntitySet<Culture>("Cultures");
        builder.EntitySet<Model.Resource>("Resources");
        builder.EntitySet<Translation>("Translations");
        builder.EntityType<Model.Resource>().Collection
            .Action("Reload");
        builder.EntityType<Model.Resource>().Collection
            .Action("Collect");
    }
}