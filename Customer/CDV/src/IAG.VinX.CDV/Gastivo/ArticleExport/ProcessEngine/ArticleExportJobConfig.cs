using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.ProcessEngine;

public class ArticleExportJobConfig : GastivoBaseJobConfig<ArticleExportJob>
{
    public string ImageUrlTemplate { get; set; } = "$$imageUrlTemplate$";
}