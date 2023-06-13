namespace IAG.Infrastructure.ImportExport;

public interface IImportExportData
{
    string Type { get; set; }
    void CheckType();
}