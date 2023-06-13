using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.Zweifel.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // jobs
        AddTemplate(ResourceIds.ExportBaseDataJobName, "de", "MySign Export Stammdaten");
        AddTemplate(ResourceIds.ExportCustomerJobName, "de", "MySign Export Kundendaten");
        AddTemplate(ResourceIds.ExportStockJobName, "de", "MySign Export Bestände");
        AddTemplate(ResourceIds.ImportJobName, "de", "MySign Import Aufträge");
            
        AddTemplate(ResourceIds.ObInsertOrderError, "de", "Error inserting order {0}: {1}");
    }
}