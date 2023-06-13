using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Formatter;
using IAG.VinX.Greiner.EslManager.Dto;

namespace IAG.VinX.Greiner.EslManager.BusinessLogic;

public class DataExtractor : IDataExtractor
{
    private readonly ISybaseConnection _connection;

    public DataExtractor(ISybaseConnection connection)
    {
        _connection = connection;
    }

    public Articles ExtractArticles()
    {
        var articles = _connection.GetQueryable<Article>().ToList();
        var gtins = _connection.GetQueryable<GtinGroup>().ToList();
        var processedArticles = articles.GroupJoin(gtins, art => art.ArtId, gtin => gtin.ArtId,
            (art, artGtins) => new ExportArticle
            {
                ArtNr = art.ArtNr,
                Description = art.Description,
                Price = art.Price,
                PromotionPrice = art.PromotionPrice,
                Category = art.Category,
                ArticleGroup = art.ArticleGroup,
                Deposit = Roundings.SwissCommercialRound(art.Deposit),
                Content = art.Content,
                Gtins = new List<long>(artGtins.Select(s => s.Gtin)),
                TextElements = new List<TextElement>
                {
                    new() {Key = "GG_Name", Caption = "Grossgebinde", Value = art.PackageName},
                    new() {Key = "GG_Inhalt", Caption = "Inhalt", Value = art.PackageContent.ToString("0.00", CultureInfo.InvariantCulture)},
                    new() {Key = "GG_Pfand", Caption = "Pfand", Value = Roundings.SwissCommercialRound(art.PackageDeposit).ToString("0.00", CultureInfo.InvariantCulture)},
                    new() {Key = "Art_Mwst", Caption = "MWST", Value = art.TaxRate.ToString("0.00", CultureInfo.InvariantCulture)}
                }
            });

        return new Articles {Items = processedArticles.ToList()};
    }

    public ArticlesDel ExtractArticlesToDelete()
    {
        var articlesDel = _connection.GetQueryable<ArticleDel>().ToList();
        return new ArticlesDel {Items = articlesDel.ToList()};
    }
}