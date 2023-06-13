using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;

public interface IArticleExporter
{
    void SetConfig(GastivoFtpConfig gastivoFtpConfig, string connectionString, string imageUrlTemplate, IMessageLogger messageLogger);
    
    GastivoExportJobResult ExportArticles();
}