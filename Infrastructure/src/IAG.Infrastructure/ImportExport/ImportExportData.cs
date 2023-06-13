using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.ImportExport;

public abstract class ImportExportData : IImportExportData
{
    public string Type { get; set; } 

    public ImportExportData()
    {
        Type = GetType().FullName;
    }

    public void CheckType()
    {
        if (Type != GetType().FullName)
        {
            throw new LocalizableException(ResourceIds.ImportExportWrongTypeExceptionMessage, GetType().FullName, Type);
        }
    }
}