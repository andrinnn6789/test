// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadBaseData;

public class UploadBaseDataJobResult : SvBaseJobResult
{
    public int BrandsCount { get; set; }

    public int CatalogCount { get; set; }

    public int ArticlesCount { get; set; }

    public int CustomersCount { get; set; }

    public int LivCount { get; set; }
}