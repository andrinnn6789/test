using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Formatter;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.ArticleExport.Dto;
using IAG.VinX.CDV.Gastivo.Common.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Resource;

using NHibernate;

using Article = IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain.Article;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;

public class ArticleExporter : IArticleExporter
{
    private readonly ISessionContextFactory _sessionContextFactory;
    private readonly IFtpConnector _ftpConnector;
    private ISession _session;
    private IMessageLogger _messageLogger;
    private string _imageUrlTemplate;

    private const string LanguageCodeGerman = "de";
    private const string LanguageCodeFrench = "fr";
    private const string LanguageCodeItalian = "it";
    
    public ArticleExporter(ISessionContextFactory sessionContextFactory, IFtpConnector ftpConnector)
    {
        _sessionContextFactory = sessionContextFactory;
        _ftpConnector = ftpConnector;
    }
    
    public void SetConfig(
        GastivoFtpConfig gastivoFtpConfig,
        string connectionString,
        string imageUrlTemplate,
        IMessageLogger messageLogger)
    {
        _messageLogger = messageLogger;
        _imageUrlTemplate = imageUrlTemplate;
        _session = _sessionContextFactory.CreateSessionContext(connectionString, MappingHelper.GetMappings(), true).Session;
        _ftpConnector.SetConfig(gastivoFtpConfig);
    }

    public GastivoExportJobResult ExportArticles()
    {
        var result = new GastivoExportJobResult();
        
        try
        {
            var articleModels = GetArticles();
            var articles = MapToArticleDto(articleModels);
            SerializeAndUpload(articles);
            result.ExportedCount = articles.Count;
        }
        catch (Exception e)
        {
            result.ErrorCount++;
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoArticleExportError, e);
            ProcessErrorLogger.Log(_messageLogger, _session, ResourceIds.GastivoArticleExportErrorTitle, string.Format(ResourceIds.GastivoArticleExportError, e));
        }
        finally
        {
            _session.Dispose();
        }

        result.Result = result.ErrorCount > 0 ? JobResultEnum.Failed : JobResultEnum.Success;

        return result;
    }

    private IEnumerable<Article> GetArticles()
    {
        return _session.Query<Article>()
            .Where(a => a.ArticleNumber.HasValue) // Nur Artikel mit Nummer
            .Where(a => a.SalesPrices
                .Any(s => 
                    s.IsActive &&
                    s.PriceGroup.Id == 3 &&
                    (!s.ValidFrom.HasValue || DateTime.Now.Date >=s.ValidFrom.Value.Date) &&
                    (!s.ValidTo.HasValue || DateTime.Now.Date <= s.ValidTo.Value.Date))) // Gültiger Preis in Preisgruppe Gastronomie
            .Where(a => a.Cycle.Id == 1 || a.Cycle.Id == 84) // Zyklus "Aktiv" und "Letzte Fl. eines Jahrgangs"
            .Where(a => a.Filling != null) // Nur mit Abfüllung
            .Where(a => a.DivisionId == null || a.DivisionId.Value == 1) // Bereich leer oder Casa
            .Where(a => a.ArticleType == 2 || a.ArticleType == 3 || a.ArticleType == 4 || a.ArticleType == 5) // Artikeltypen: 2=Wein, 3=Bier, 4=Spirituosen, 5=Mineral & Fruchtsäfte
            .Where(a => a.ECommerceGroup != null) // Nur mit E-Commerce Gruppe
            .ToList();
    }

    private List<Dto.Article> MapToArticleDto(IEnumerable<Article> articleModels)
    {
        return articleModels
            .Select(articleModel =>
                new Dto.Article
                {
                    ArticleNumber = articleModel.ArticleNumber!.Value.ToString("#"),
                    ArticleTextLong = CreateTranslationsForArticleText(articleModel),
                    EanCode = string.Join(",", new[] { articleModel.EanCode1, articleModel.EanCode2, articleModel.EanCode3, articleModel.EanCode4 }
                            .Where(s => s.HasValue)
                            .Select(s => s.Value.ToString("#"))),
                    ArticleUnitCode = articleModel.Filling!.AbbreviationForWeb,
                    ArticleUnitText = CreateTranslationsForArticleUnit(articleModel.Filling!.AbbreviationForWeb),
                    SellAmount = 1,
                    VatRatePercent = articleModel.Vat.Percentage,
                    Price = articleModel.SalesPrices
                        .First(p => 
                            p.IsActive && 
                            p.PriceGroup.Id == 3 && 
                            (!p.ValidFrom.HasValue || DateTime.Now.Date >= p.ValidFrom.Value.Date) && 
                            (!p.ValidTo.HasValue || DateTime.Now.Date <= p.ValidTo.Value.Date))
                        .Price,
                    ArticleType = ArticleType.Piece,
                    DaysToDeliver = 1,
                    StockType = GetArticleAvailability(articleModel),
                    ProductCategoryLvl1 = articleModel.ArticleType,
                    ProductCategoryLvl2 = articleModel.Category?.Id,
                    ImageUrl = string.Format(_imageUrlTemplate, articleModel.Id.ToString()),
                    WineType = CreateTranslations(articleModel.Category?.Description, articleModel.Category?.DescriptionFrench, articleModel.Category?.DescriptionItalian, 100),
                    WineRegion = CreateTranslations(articleModel.Region?.Name, articleModel.Region?.NameFrench, articleModel.Region?.NameItalian, 100),
                    RawMaterialOriginCode = articleModel.Country?.IsoNumber != null && articleModel.Country?.IsoNumber != 0 ? articleModel.Country?.IsoNumber : null,
                    ArticleDescriptionText = CreateTranslations(articleModel.WineInfo?.Character, articleModel.WineInfo?.CharacterFrench, articleModel.WineInfo?.CharacterItalian, 2000),
                    FitsWellWithDetail = CreateTranslations(
                        articleModel.WineInfo?.Recommendations?.Select(r => r.Description).ToList(), 
                        articleModel.WineInfo?.Recommendations?.Select(r => r.DescriptionFrench).ToList(), 
                        articleModel.WineInfo?.Recommendations?.Select(r => r.DescriptionItalian).ToList(),
                        1000),
                    Grapes = CreateTranslations(
                        articleModel.WineInfo?.Grapes?.Select(r => r.Description).ToList(), 
                        articleModel.WineInfo?.Grapes?.Select(r => r.DescriptionFrench).ToList(), 
                        articleModel.WineInfo?.Grapes?.Select(r => r.DescriptionItalian).ToList(),
                        100)
                })
            .ToList();
    }

    private void SerializeAndUpload(IEnumerable<Dto.Article> articles)
    {
        var serializedData = GastivoSerializationHelper.SerializeAsJson(articles);
        _ftpConnector.UploadFile(serializedData, $"articles-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.json");
    }
    
    private static StockType GetArticleAvailability(Article article)
    {
        // Calculate availability for warehouses "Rupperswil", "Depot Birrhard" & "Ligornetto (Ligo CdV)"
        var currentAvailability = article.Stocks
            .Where(s => s.Warehouse.Id is 1 or 9 or 786)
            .Sum(s => s.OnStock - s.Reserved - s.Provision);
        
        var minimumStock = article.Stocks
            .Where(s => s.Warehouse.Id is 1 or 9 or 786)
            .Sum(s => s.MininumStockWebshop);

        return currentAvailability > 0 && currentAvailability >= minimumStock
            ? StockType.Available
            : StockType.OnRequest;
    }
    
    private static List<Translation> CreateTranslationsForArticleText(Article article)
    {
        var articleDescriptionGerman = CreateArticleDescription(article.ProductTitle ?? article.Description, article.Filling?.Abbreviation);
        var articleDescriptionFrench = CreateArticleDescription(article.ProductTitleFrench, article.Filling?.Abbreviation);
        var articleDescriptionItalian = CreateArticleDescription(article.ProductTitleItalian, article.Filling?.Abbreviation);

        return CreateTranslations(articleDescriptionGerman, articleDescriptionFrench, articleDescriptionItalian, 100);
    }
    
    private static string CreateArticleDescription(string description, string filling)
    {
        if (string.IsNullOrEmpty(description))
            return string.Empty;

        var articleDescription = description.TrimEnd();
        
        if (!string.IsNullOrEmpty(filling))
            articleDescription += $", {filling}";

        return articleDescription;
    }

    private static List<Translation> CreateTranslationsForArticleUnit(string articleUnitCode)
    {
        return articleUnitCode switch
        {
            "KGM" => CreateTranslations("Kilogramm", "Kilogramme", "Chilogrammo", 50),
            "LTR" => CreateTranslations("Liter", "Litres", "Litri", 50),
            "PCE" => CreateTranslations("Stück", "Pièce", "Pezzo", 50),
            "PK" => CreateTranslations("Pack", "Paquet", "Pacco", 50),
            _ => CreateTranslations("Flasche", "Bouteille", "Bottiglia", 50)
        };
    }
    
    private static List<Translation> CreateTranslations(IReadOnlyCollection<string> textsInGerman, IReadOnlyCollection<string> textsInFrench, IReadOnlyCollection<string> textsInItalian, int maximumSize)
    {
        if (textsInGerman == null || !textsInGerman.Any(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)))
            return null;

        var joinedTextInGerman = string.Join(", ", 
            textsInGerman.Where(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)));
        
        var joinedTextInFrench =
            textsInFrench != null && textsInFrench.Any(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t))
                ? string.Join(", ", textsInFrench.Where(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)))
                : string.Empty;
        var joinedTextInItalian =
            textsInItalian != null && textsInItalian.Any(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t))
                ? string.Join(", ", textsInItalian.Where(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)))
                : string.Empty;

        return CreateTranslations(joinedTextInGerman, joinedTextInFrench, joinedTextInItalian, maximumSize);
    }

    private static List<Translation> CreateTranslations(string textInGerman, string textInFrench, string textInItalian, int maximumSize)
    {
        if (string.IsNullOrEmpty(textInGerman))
            return null;

        var translationGerman = CreateTranslation(LanguageCodeGerman, textInGerman, maximumSize);
        var translationFrench = CreateTranslation(LanguageCodeFrench, textInFrench, maximumSize);
        var translationItalian = CreateTranslation(LanguageCodeItalian, textInItalian, maximumSize);

        var translations = new List<Translation>() { translationGerman };
        
        if(translationFrench != null)
            translations.Add(translationFrench);
        
        if(translationItalian != null)
            translations.Add(translationItalian);

        return translations;
    }
    
    private static Translation CreateTranslation(string languageCode, string translation, int maximumSize)
    {
        if (string.IsNullOrEmpty(translation))
            return null;
        
        return new Translation()
        {
            Lang = languageCode,
            Value = TransformText(translation, maximumSize)
        };
    }
    
    private static string TransformText(string source, int maximumSize)
    {
        // Remove RTF tags
        var rtfCleanedSource = RtfCleaner.Clean(source);
        
        // Replace Linebreaks
        var transformedSource = rtfCleanedSource.Replace(Environment.NewLine, ", ").Trim();
        
        // Truncate to maximum size
        return transformedSource.Length <= maximumSize ? transformedSource : transformedSource[..maximumSize];
    }
}