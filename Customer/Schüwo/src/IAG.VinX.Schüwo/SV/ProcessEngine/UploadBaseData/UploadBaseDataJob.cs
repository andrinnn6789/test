using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.Resource;

namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadBaseData;

[JobInfo("02407655-3773-485C-9334-DA0B15678A7B", JobName)]
public class UploadBaseDataJob : SvBaseJob<UploadBaseDataJobConfig, JobParameter, UploadBaseDataJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SVUploadBaseData";

    public UploadBaseDataJob(ISybaseConnectionFactory sybaseConnectionFactory)
        : base(sybaseConnectionFactory)
    {
    }

    protected override void Sync()
    {
        Result.BrandsCount = FormatAndUploadData(Extractor.ExtractBrands(), Config.FtpPathConfig.BrandDataName);
        Result.CatalogCount = FormatAndUploadData(Extractor.ExtractCatalog(), Config.FtpPathConfig.CatalogDataName);
        Result.ArticlesCount = FormatAndUploadData(Extractor.ExtractArticles(), Config.FtpPathConfig.ArticleDataName);
        Result.CustomersCount = FormatAndUploadData(Extractor.ExtractCustomers(), Config.FtpPathConfig.CustomerDataName);
        Result.LivCount = FormatAndUploadData(Extractor.ExtractArtAdditional(), Config.FtpPathConfig.ArtAdditionalDataName);
    }
}