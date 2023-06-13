using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using IAG.VinX.Basket.Enum;
using IAG.VinX.OrderBravo.BusinessLogic.Interface;
using IAG.VinX.OrderBravo.Dto;
using IAG.VinX.OrderBravo.Dto.Interfaces;
using IAG.VinX.Zweifel.OrderBravo.Dto;

namespace IAG.VinX.Zweifel.OrderBravo.BusinessLogic;

public class ObzCatalogMapper : IObsCatalogMapper
{
    private ObsCatalogProduct _catalogProduct;
    private ObzArticle _obVinXArticle;
    private const string DefaultCurrency = "CHF";
    private const string DefaultUnit = "Flasche";
    private const string AvailabilityDiscontinued = "discontinued";
    private const string AvailabilityInStock = "in_stock";

    public void SetGlobalMinimumStockAmount(int amount)
    {
    }

    public void SetCategorySetting(OrderBravoCategorySetting categorySetting)
    {
    }

    public ObsCatalogProduct MapArticle(IObArticle obArticle)
    {
        _obVinXArticle = (ObzArticle)obArticle;
        _catalogProduct = new ObsCatalogProduct
        {
            productCode = Convert.ToInt32(_obVinXArticle.ArticleNumber).ToString(),
            name = ComposeName(),
            category = ComposeArticleType(_obVinXArticle.ArticleType),
            subcategory = _obVinXArticle.ArticleCategory,
            currency = DefaultCurrency
        };

        SetArticleUnits();
        SetAvailabilityAndLeadtime();
        SetReplacementArticles();
        SetCustomerNumbers();
        return _catalogProduct;
    }

    private void SetCustomerNumbers()
    {
        if (_obVinXArticle.CustomerNumbers != null)
        {
            var customerNumbers = ParseCustomerNumbers(_obVinXArticle.CustomerNumbers);
            _catalogProduct.customerNumbers = customerNumbers;
        }
    }

    private void SetReplacementArticles()
    {
        if (_obVinXArticle.ReplacementArticleNumber != null)
            _catalogProduct.replacementProducts = new List<string>
            {
                Convert.ToInt32(_obVinXArticle.ReplacementArticleNumber).ToString()
            };
    }

    private void SetAvailabilityAndLeadtime()
    {
        if ((_obVinXArticle.StockAmount < 6 && _obVinXArticle.CycleId == 112) ||
            (_obVinXArticle.StockAmount < 6 && _obVinXArticle.CycleId == 5))
            _catalogProduct.availability = AvailabilityDiscontinued;
        else
            _catalogProduct.availability = AvailabilityInStock;

        if (_obVinXArticle.CycleId == 4) _catalogProduct.hasLeadTime = true;
    }

    private void SetArticleUnits()
    {
        _catalogProduct.units = new List<ObsUnit>();

        if (!string.IsNullOrWhiteSpace(_obVinXArticle.PackingUnitDescription))
            _catalogProduct.units.Add(
                new ObsUnit
                {
                    unit = _obVinXArticle.PackingUnitDescription,
                    unitCode = _obVinXArticle.PackingUnitId.ToString(),
                    measurementUnitQuantity = _obVinXArticle.PackingUnitQtyPerUnit
                });

        if (!_obVinXArticle.PackingUnitOnly &&
            _obVinXArticle.PackingUnitBreakageAllowed &&
            _obVinXArticle.ArticleType != ArticleCategoryKind.Set &&
            _obVinXArticle.ArticleType != ArticleCategoryKind.Diverse &&
            _obVinXArticle.ArticleType != ArticleCategoryKind.Food)
            _catalogProduct.units.Add(
                new ObsUnit
                {
                    unit = DefaultUnit,
                    unitCode = _obVinXArticle.FillingId.ToString(),
                    measurementUnitQuantity = 1
                });

        if (!_catalogProduct.units.Any())
            _catalogProduct.units.Add(
                new ObsUnit
                {
                    unit = "Bezeichnung nicht erfasst",
                    unitCode = "",
                    measurementUnitQuantity = 0
                });
    }

    private string ComposeName()
    {
        string name;
        switch (_obVinXArticle.ArticleType)
        {
            case ArticleCategoryKind.Wine:
                if (!_obVinXArticle.PackingUnitBreakageAllowed)
                    name = _obVinXArticle.ArticleDescription;
                else
                    name = _obVinXArticle.ArticleDescription + " " + _obVinXArticle.Year + " " +
                           _obVinXArticle.ProducerName + " " +
                           _obVinXArticle.FillingDescription;

                break;
            case ArticleCategoryKind.Spirits:
                if (!_obVinXArticle.PackingUnitBreakageAllowed)
                    name = _obVinXArticle.ArticleDescription;
                else
                    name = _obVinXArticle.ArticleDescription + " " + Convert.ToInt32(_obVinXArticle.Volume) + "%" +
                           " " +
                           _obVinXArticle.ProducerName + " " + _obVinXArticle.FillingDescription;

                break;
            case ArticleCategoryKind.Mineral:
                name = _obVinXArticle.ArticleDescription + " " + _obVinXArticle.FillingWebDescription;
                break;
            case ArticleCategoryKind.Beer:
                if (_obVinXArticle.PackingUnitDescription == "Tank")
                {
                    name = _obVinXArticle.ArticleDescription + " " + _obVinXArticle.PackingUnitDescription;
                }
                else
                {
                    if (!_obVinXArticle.PackingUnitBreakageAllowed)
                        name = _obVinXArticle.ArticleDescription;
                    else
                        name = _obVinXArticle.ArticleDescription + " " + _obVinXArticle.FillingWebDescription;
                }

                break;
            default:
                name = _obVinXArticle.ArticleDescription;
                break;
        }

        return name;
    }

    [ExcludeFromCodeCoverage]
    private string ComposeArticleType(ArticleCategoryKind kind)
    {
        switch (kind)
        {
            case ArticleCategoryKind.Wine:
                return "Wein";
            case ArticleCategoryKind.Mineral:
                return "Mineral und Fruchtsäfte";
            case ArticleCategoryKind.Beer:
                return "Bier";
            case ArticleCategoryKind.Spirits:
                return "Spirituosen";
            case ArticleCategoryKind.Diverse:
                return "Diverse";
            case ArticleCategoryKind.Service:
                return "Dienstleistungen";
            case ArticleCategoryKind.Set:
                return "Verkaufs-Set";
            case ArticleCategoryKind.Food:
                return "Lebensmittel";
            default:
                return null;
        }
    }

    private IList<string> ParseCustomerNumbers(string customerNumbers)
    {
        return customerNumbers.Split(',').ToList();
    }
}