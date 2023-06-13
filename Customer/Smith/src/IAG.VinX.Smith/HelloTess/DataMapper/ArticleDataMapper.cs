using System;

using IAG.Infrastructure.Atlas;
using IAG.VinX.Smith.HelloTess.Dto;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

namespace IAG.VinX.Smith.HelloTess.DataMapper;

class ArticleDataMapper : IDataMapper<VxArticle, HelloTessRest.Dto.Article>
{
    private readonly SyncSystemDefaults _syncSystemDefaults;

    public ArticleDataMapper(SyncSystemDefaults syncSystemDefaults)
    {
        _syncSystemDefaults = syncSystemDefaults;
    }

    public bool CheckUpdate(VxArticle source, HelloTessRest.Dto.Article target)
    {
        var name = source.Bezeichnung;
        var description = _syncSystemDefaults.ArticleDescriptionPrefix + source.Artikelnummer;
        var plu = Convert.ToInt64(_syncSystemDefaults.ArticlePluPrefix + source.Artikelnummer);
        var articleGroupId = new GuidConverter().ToBigEndianGuid(source.ArtikelkategorieGuid).ToString();
        var price = Convert.ToInt32(source.InhousePreis * 100).ToString();
        var taxRate = Convert.ToInt32(source.MwstProzent * 100);
        var netProductionCosts = Convert.ToInt32(source.PriceGroupPrice * 100);
        var barcode = source.EanProEinheit == 0.0 ? null : source.EanProEinheit.ToString();

        // check if data has changed
        if (target.Name == name &&
            target.Description == description &&
            target.Plu == plu &&
            target.ArticleGroupId == articleGroupId &&
            target.Price == price &&
            target.TaxInhouse == taxRate.ToString() &&
            target.TaxTakeaway == taxRate &&
            target.NetProductionCosts == netProductionCosts &&
            target.Barcode == barcode
           ) return false;

        target.Name = name;
        target.Description = description;
        target.Plu = plu;
        target.ArticleGroupId = articleGroupId;
        target.Price = price;
        target.TaxInhouse = taxRate.ToString();
        target.TaxTakeaway = taxRate;
        target.NetProductionCosts = netProductionCosts;
        target.Barcode = barcode;
        return true;
    }

    public HelloTessRest.Dto.Article CreateTarget(VxArticle source)
    {
        return new()
        {
            Id = new GuidConverter().ToBigEndianGuid(source.Guid).ToString(),
            Name = source.Bezeichnung,
            Description = _syncSystemDefaults.ArticleDescriptionPrefix + source.Artikelnummer,
            Plu = Convert.ToInt64(_syncSystemDefaults.ArticlePluPrefix + source.Artikelnummer),
            ArticleGroupId = new GuidConverter().ToBigEndianGuid(source.ArtikelkategorieGuid).ToString(),
            Price = Convert.ToInt32(source.InhousePreis * 100).ToString(),
            TaxInhouse = Convert.ToInt32(source.MwstProzent * 100).ToString(),
            TaxTakeaway = Convert.ToInt32(source.MwstProzent * 100),
            NetProductionCosts = Convert.ToInt32(source.PriceGroupPrice * 100),
            Type = 0,
            InheritColors = true,
            InheritTaxrate = true,
            ShowInMenu = true,
            Barcode = source.EanProEinheit == 0.0 ? null : source.EanProEinheit.ToString()
        };
    }

    public bool CheckDelete(HelloTessRest.Dto.Article target)
    {
        if (target?.Plu == null) return false;
        // only VinX-article starts with the specific prefix, others are ignored
        if (!target.Plu.Value.ToString().StartsWith(_syncSystemDefaults.ArticlePluPrefix)) return false;
        if (target.ShowInMenu == false && target.Barcode == null) return false;

        target.ShowInMenu = false;
        target.Barcode = null;
        return true;
    }
}