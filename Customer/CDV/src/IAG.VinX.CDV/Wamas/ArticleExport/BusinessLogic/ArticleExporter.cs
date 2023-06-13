using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.ArticleExport.Dto;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

using Article = IAG.VinX.CDV.Wamas.ArticleExport.Dto.Article;

namespace IAG.VinX.CDV.Wamas.ArticleExport.BusinessLogic;

public class ArticleExporter : BaseExporter, IArticleExporter
{ 
    public ArticleExporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Type[] RecordTypes => new[]
    {
        typeof(Article),
        typeof(ArticleQuantityUnit), typeof(ArticleDescription), typeof(ArticleAlias)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasExportJobResult ExportArticles(DateTime lastSync)
    {
        var jobResult = new WamasExportJobResult();

        try
        {
            var records = GetRecords(lastSync);

            if (records.Any())
                SerializeAndUpload(records, ResourceIds.WamasArticleRecordType);

            jobResult.Result = JobResultEnum.Success;
            jobResult.ExportedCount = records.Count;
        }
        catch (Exception e)
        {
            jobResult.Result = JobResultEnum.Failed;
            jobResult.ErrorCount++;
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasArticleExportError, e.Message);
            
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasArticleExportErrorTitle,
                string.Format(ResourceIds.WamasArticleExportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private List<GenericWamasRecord> GetRecords(DateTime lastSync)
    {
        var records = new List<GenericWamasRecord>();
        var articles = DatabaseConnector.GetQueryable<Article>().Where(a => a.ChangedOn >= lastSync).ToList();
        var articleQuantityUnits = DatabaseConnector.GetQueryable<ArticleQuantityUnit>().Where(a => a.ChangedOn >= lastSync).ToList();
        var articleDescriptions = DatabaseConnector.GetQueryable<ArticleDescription>().Where(a => a.ChangedOn >= lastSync).ToList();
        var articleAliases = DatabaseConnector.GetQueryable<ArticleAlias>().Where(a => a.ChangedOn >= lastSync).ToList();

        foreach (var article in articles)
        {
            records.Add(new GenericWamasRecord(article.GetType(), article));

            var quantityUnitsForArticle = articleQuantityUnits.Where(a => a.ItemNumber == article.ItemNumber).ToList();
            foreach (var articleQuantityUnit in quantityUnitsForArticle)
            {
                records.Add( new GenericWamasRecord(articleQuantityUnit.GetType(), articleQuantityUnit));
            }

            var descriptionsForArticle = articleDescriptions.Where(a => a.ItemNumber == article.ItemNumber).ToList();
            foreach (var articleDescription in descriptionsForArticle)
            {
                records.Add(new GenericWamasRecord(articleDescription.GetType(), articleDescription));
            }

            var aliasesForArticle = articleAliases.Where(a => a.ItemNumber == article.ItemNumber).ToList();
            foreach (var articleAlias in aliasesForArticle)
            {
                records.Add(new GenericWamasRecord(articleAlias.GetType(), articleAlias));
            }
        }

        return records;
    }
}