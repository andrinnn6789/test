namespace IAG.VinX.Schüwo.SV.Config;

public class FtpPathConfig
{
    public string WorkingDir { get; set; } = "/fromdist/incoming";

    public string FinalDir { get; set; } = "/fromdist";

    public string ImageDir { get; set; } = "/fromdist/images";

    public string DownloadOrderDir { get; set; } = "/todist";

    public string Extension { get; set; } = ".csv";

    public string BrandDataName { get; set; } = "bran";

    public string CatalogDataName { get; set; } = "catd";

    public string ArticleDataName { get; set; } = "arts";

    public string CustomerDataName { get; set; } = "cust";

    public string ArtAdditionalDataName { get; set; } = "adtl";

    public string ArchiveOrdersDataName { get; set; } = "ords";
}