using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Formatter;
using IAG.Infrastructure.Globalisation.Validator;
using IAG.Infrastructure.Logging;
using IAG.VinX.BaseData.Dto;
using IAG.VinX.ExternalDataProvider.VinXDbAccess.Liv;
using IAG.VinX.Schüwo.SV.Dto;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class DataExtractor
{
    private readonly ISybaseConnection _connection;
    private readonly IMessageLogger _msgLogger;
    private readonly ResultCounts _resultCounts;
    private readonly UnitMapper _unitMapper;

    public DataExtractor(ISybaseConnection connection, IMessageLogger msgLogger, ResultCounts resultCounts)
    {
        _connection = connection;
        _msgLogger = msgLogger;
        _resultCounts = resultCounts;
        _unitMapper = new UnitMapper(msgLogger, resultCounts);
    }

    #region public

    public List<Brand> ExtractBrands() => _connection.GetQueryable<Brand>().ToList();

    public List<CatalogEntry> ExtractCatalog() => _connection.GetQueryable<CatalogEntry>().ToList();

    public List<UploadArticle> ExtractArticles()
    {
        var vxArticles = _connection.GetQueryable<ArticleSw>().ToList();
        var svArticles = new List<UploadArticle>();
        var sb = new StringBuilder();
        vxArticles.ForEach(art =>
        {
            sb.Clear();
            if (!string.IsNullOrWhiteSpace(art.Vintage))
                sb.Append($"Jahrgang {art.Vintage}, ");
            if (art.Barrique == 1)
                sb.Append("Barriqueausbau, ");
            if (!string.IsNullOrWhiteSpace(art.Vinification))
                sb.Append($"Vinifikation: {art.Vinification} ");
            if (!string.IsNullOrWhiteSpace(art.Charakter))
                sb.Append($"Charakter: {art.Charakter} ");
            if (!string.IsNullOrWhiteSpace(art.Rating))
                sb.Append($"Bewertung: {art.Rating} ");
            if (!string.IsNullOrWhiteSpace(art.ConsumationHint))
                sb.Append($"Konsumhinweis: {art.ConsumationHint} ");
            if (!string.IsNullOrWhiteSpace(art.Grapes))
                sb.Append($"Traubensorten: {art.Grapes} ");
            if (!string.IsNullOrWhiteSpace(art.Terroir))
                sb.Append($"Terroir: {art.Terroir} ");

            svArticles.Add(new UploadArticle
            {
                Anbr = art.ArtNr.ToString(),
                Status = 1,
                Text_De = TrimField(art.Description),
                Origin = ValidateCountry(art.CountryIso),
                Brand = art.BrandId.ToString(),
                Unit1 = art.SellAsUnit ?_unitMapper.GetUnit1(art) : _unitMapper.GetUnit2(art),
                Price1 = art.Price1,
                Deposit1 = art.SellAsUnit ? art.DepositPriceUnit : art.DepositPriceUnit * art.UnitsPerBulkPackaging + art.DepositPriceContainer,
                Unit2 = art.SellAsUnit ? _unitMapper.GetUnit2(art) : null,
                Price2 = art.SellAsUnit ? art.Price * art.UnitsPerBulkPackaging : null,
                Deposit2 = art.SellAsUnit ? art.DepositPriceContainer: null,
                CatId1 = art.ArtEGroupId.ToString(),
                Longtext_De = RtfToText(sb.ToString()).TrimEnd(','),
                Vat = art.Vat,
                Minord = art.SellAsUnit ? string.Empty : $"1{_unitMapper.MapUnit(art.BulkPackagingTextShort)}",
                Codes1 = art.SellAsUnit ? art.EanUnit : art.EanBulk,
                Codes2 = art.SellAsUnit ? art.EanBulk : string.Empty
            });
        });

        return svArticles;
    }
    public List<int> GetActiveArticles()
    {
        return _connection.GetQueryable<ArticleActive>().ToList().Select(a => a.ArtNr).ToList();
    }

    public List<UploadArtAdditional> ExtractArtAdditional()
    {
        var vxArticles = _connection.GetQueryable<ArticleSw>().ToList();
        var vxLiv = _connection.GetQueryable<ArticleLivBase>().ToList();
        var artAdditionals = new List<UploadArtAdditional>();
        var livMapperDd = new LivMapper();
        vxLiv.ForEach(art =>
        {
            var livData = livMapperDd.Transform(art.DdContent);
            artAdditionals.Add(new UploadArtAdditional
            {
                _anbr = art.Artikelnummer.ToString(),
                _status = 1,
                sv_standardprice = vxArticles.FirstOrDefault(a => a.ArtNr == art.Artikelnummer)?.Price1,
                buying_currency = "CHF",
                contains_alkohol = livData.LivDatas[LivTypeEnum.AlcoholContent].Contains,
                contains_caffeine = livData.LivDatas[LivTypeEnum.Caffeine].Contains,
                contains_egg = livData.LivDatas[LivTypeEnum.Eggs].Contains,
                contains_fisch = livData.LivDatas[LivTypeEnum.Fish].Contains,
                contains_gluten = livData.LivDatas[LivTypeEnum.Gluten].Contains,
                contains_lactose = livData.LivDatas[LivTypeEnum.Lactose].Contains,
                contains_lupins = livData.LivDatas[LivTypeEnum.Lupine].Contains,
                contains_nuts = livData.LivDatas[LivTypeEnum.Nuts].Contains,
                contains_moustard = livData.LivDatas[LivTypeEnum.Mustard].Contains,
                contains_peanuts = livData.LivDatas[LivTypeEnum.Peanuts].Contains,
                contains_sellerie = livData.LivDatas[LivTypeEnum.Celery].Contains,
                contains_sesame = livData.LivDatas[LivTypeEnum.Sesame].Contains,
                contains_sulfur_dioxide_sulfites = livData.LivDatas[LivTypeEnum.Sulphites].Contains,
                contains_taurine = livData.LivDatas[LivTypeEnum.Taurin].Contains,
                diet_vegetarian = true,
                diet_vegan = true,
                ingredient_list_De = RtfToText(art.Zutatenliste),
                manufacturer = art.Produzent,
                manufacturing_country = ValidateCountry(art.Produktionsland),
                manufacturing_local_region = art.Region,
                nutritional_value_amount = ReferceQty(livData.LivDatas[LivTypeEnum.EnergyKcal].MeasureUnit),
                nutritional_value_unit = ReferceUnit(livData.LivDatas[LivTypeEnum.EnergyKcal].MeasureUnit),
                nutritional_value_calories = livData.LivDatas[LivTypeEnum.EnergyKcal].Value,
                nutritional_value_joules = livData.LivDatas[LivTypeEnum.EnergyKj].Value,
                nutritional_value_carbohydrates = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.Carbohydrate].MeasureUnit) * livData.LivDatas[LivTypeEnum.Carbohydrate].Value,
                nutritional_value_fat = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.Fat].MeasureUnit) * livData.LivDatas[LivTypeEnum.Fat].Value,
                nutritional_value_protein = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.Protein].MeasureUnit) * livData.LivDatas[LivTypeEnum.Protein].Value,
                nutritional_value_salt = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.Salteq].MeasureUnit) * livData.LivDatas[LivTypeEnum.Salteq].Value,
                nutritional_value_saturated_fatty_acids = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.FattyAcidsSaturated].MeasureUnit) * livData.LivDatas[LivTypeEnum.FattyAcidsSaturated].Value,
                nutritional_value_sugar = ReferceFactorForMg(livData.LivDatas[LivTypeEnum.Sugar].MeasureUnit) * livData.LivDatas[LivTypeEnum.Sugar].Value,
                supplier = "Schüwo",
                supply_country = "CH"
            });
        });

        return artAdditionals;
    }

    public List<UploadCustomer> ExtractCustomers()
    {
        var vxCustomers = _connection.GetQueryable<Customer>().ToList();
        var svCustomers = new List<UploadCustomer>();
        vxCustomers.ForEach(c =>
        {
            svCustomers.Add(new UploadCustomer
            {
                Cid = c.AdrNr.ToString(),
                Ocid = c.Uid,
                Status = 1,
                Name = c.Name,
                Addr1 = c.AdrAddition1,
                Addr2 = c.Street,
                Zip = c.Zip,
                Loc = c.Town,
                Tel = c.TelG,
                Fax = c.FaxG,
                Country = ValidateCountry(c.CountryIso),
                Cost_Centre_Id = c.SvCostCentreId,
                Tour = c.Tour,
                Otime = c.Otime,
                Channel_Filter = "sv_pricat|sv_group"
            });
        });

        return svCustomers;
    }

    public List<UploadArchiveOrder> ExtractArchiveOrders()
    {
        var lastYear = DateTime.Today.AddYears(-1);
        var vxArchiveOrders = _connection.GetQueryable<ArchiveOrder>().Where(o => o.ChangedOn > lastYear);
        var svArchiveOrders = new List<UploadArchiveOrder>();
        var svArchiveOrderPositions = ExtractArchiveOrderPositions();
        vxArchiveOrders.ToList().ForEach(order =>
        {
            var svOrderPositions = svArchiveOrderPositions.Where(orderPos => orderPos.oid == order.OrderNr.ToString()).ToList();
            if (svOrderPositions.Count > 0)
            {
                svArchiveOrders.Add(new UploadArchiveOrder
                {
                    _gpoid = order.ExtId,
                    _typecode = 4,
                    _statuscode = order.StatusPayment > 70 ? 6 : 5,
                    _oid = order.OrderNr.ToString(),
                    _cid = order.CustomerNr.ToString(),
                    _date = order.ChangedOn?.Date.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                    _executiondate = order.OrderDate.Date.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                    _poscount = svOrderPositions.Count,
                    PosData = svOrderPositions
                });
            }
        });

        return svArchiveOrders;
    }

    #endregion

    #region private

    private List<UploadOrderPos> ExtractArchiveOrderPositions()
    {
        var lastYear = DateTime.Today.AddYears(-1);
        var vxOrderPos = _connection.GetQueryable<OrderPos>()
            .Where(pos => pos.OrderStatus == 100)
            .Where(pos => pos.ChangedOn > lastYear);
        var svOrderPos = new List<UploadOrderPos>();
        vxOrderPos.ToList().ForEach(orderPos =>
        {
            svOrderPos.Add(new UploadOrderPos
            {
                oid = orderPos.OrderNr.ToString(),
                lnbrr = orderPos.OrderPosNr,
                lnbra = orderPos.OrderPosId,
                quant = orderPos.SellAsUnit ? orderPos.QuantityInFilling : orderPos.QuantityInBulkPackaging,
                quant_d = orderPos.SellAsUnit ? orderPos.QuantityInFilling : orderPos.QuantityInBulkPackaging,
                anbr = orderPos.ArtNr.ToString(),
                unit = orderPos.SellAsUnit ? _unitMapper.MapUnit(orderPos.FillingTextShort) : _unitMapper.MapUnit(orderPos.BulkPackagingTextShort),
                price = orderPos.SellAsUnit ? orderPos.Price : orderPos.Price * orderPos.UnitsPerBulkPackaging,
                total = orderPos.Price * orderPos.QuantityInFilling,
                vat = orderPos.Vat,
                arts_text_de = TrimField(orderPos.Description)
            });
        });

        return svOrderPos;
    }

    private static string RtfToText(string rtfInput)
    {
        if (string.IsNullOrEmpty(rtfInput))
            return rtfInput;

        var plainText = TrimField(RtfCleaner.Clean(rtfInput).Trim());
        return WindowsToUnicodeConverter.Convert(plainText);
    }

    private static string TrimField(string field)
    {
        return field.Replace("\r\n", " ").Replace("\n", " ").Trim();
    }

    private static int ReferceQty(LivMeasureUnit unit)
    {
        return unit switch
        {
            LivMeasureUnit.NotDef => 0,
            LivMeasureUnit.G100Ml => 100,
            LivMeasureUnit.Kcal100Ml => 100,
            LivMeasureUnit.Kj100Ml => 100,
            _ => throw new Exception($"unit not evaluated: {unit}")
        };
    }
    private static int ReferceFactorForMg(LivMeasureUnit unit)
    {
        return unit switch
        {
            LivMeasureUnit.NotDef => 0,
            LivMeasureUnit.G100Ml => 1000,
            _ => throw new Exception($"unit not evaluated: {unit}")
        };
    }
        
    private static int ReferceUnit(LivMeasureUnit unit)
    {
        return unit switch
        {
            LivMeasureUnit.NotDef => 2,
            LivMeasureUnit.G100Ml => 2,
            LivMeasureUnit.Kcal100Ml => 2,
            LivMeasureUnit.Kj100Ml => 2,
            _ => throw new Exception($"unit not evaluated: {unit}")
        };
    }

    private string ValidateCountry(string isoCode)
    {
        try
        {
            Iso3166Validator.ValidateIso2Country(isoCode);
            return isoCode;
        }
        catch (Exception e)
        {
            _msgLogger.AddMessage(e);
            _resultCounts.WarningCount++;
            return string.Empty;
        }
    }

    #endregion
}