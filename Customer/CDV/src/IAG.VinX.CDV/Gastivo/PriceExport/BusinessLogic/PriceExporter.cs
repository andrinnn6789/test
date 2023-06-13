using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.Common.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.PriceExport.Dto;
using IAG.VinX.CDV.Resource;

using NHibernate;

namespace IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;

public class PriceExporter : IPriceExporter
{
    private readonly ISessionContextFactory _sessionContextFactory;
    private readonly IFtpConnector _ftpConnector;
    private ISession _session;
    private IMessageLogger _messageLogger;
    
    public PriceExporter(ISessionContextFactory sessionContextFactory, IFtpConnector ftpConnector)
    {
        _sessionContextFactory = sessionContextFactory;
        _ftpConnector = ftpConnector;
    }
    
    public void SetConfig(
        GastivoFtpConfig gastivoFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        _messageLogger = messageLogger;
        _session = _sessionContextFactory.CreateSessionContext(connectionString, MappingHelper.GetMappings(), true).Session;
        _ftpConnector.SetConfig(gastivoFtpConfig);
    }

    public GastivoExportJobResult ExportPrices()
    {
        var result = new GastivoExportJobResult();
        
        try
        {
            var addressSpecialPriceModels = GetAddressSpecialPrices();
            var prices = MapToPricesDto(addressSpecialPriceModels);
            SerializeAndUpload(prices);
            result.ExportedCount = prices.Count;
        }
        catch (Exception e)
        {
            result.ErrorCount++;
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoPriceExportError, e);
            ProcessErrorLogger.Log(_messageLogger, _session, ResourceIds.GastivoPriceExportErrorTitle, string.Format(ResourceIds.GastivoPriceExportError, e));
        }
        finally
        {
            _session.Dispose();
        }

        result.Result = result.ErrorCount > 0 ? JobResultEnum.Failed : JobResultEnum.Success;

        return result;
    }

    private IEnumerable<AddressSpecialPricesStruct> GetAddressSpecialPrices()
    {
        var deliveryAddressesWithSpecialPrices = _session.Query<Address>()
            .Where(a => a.IsActive)
            .Where(a => a.AddressNumber.HasValue)
            .Where(a => a.TransmitToGastivo)
            .Where(a => a.PriceCondition == 10)
            .Select(a => new AddressSpecialPricesStruct(a, GetFilteredSpecialPrices(a.SpecialPrices)))
            .ToList();

        var billingAddressesWithSpecialPrices = _session.Query<Address>()
            .Where(a => a.IsActive)
            .Where(a => a.AddressNumber.HasValue)
            .Where(a => a.TransmitToGastivo)
            .Where(a => a.PriceCondition == 20)
            .Select(a => new AddressSpecialPricesStruct(a, GetFilteredSpecialPrices(a.BillingAddress.SpecialPrices)))
            .ToList();

        var conditionAddressesWithSpecialPrices = _session.Query<Address>()
            .Where(a => a.IsActive)
            .Where(a => a.AddressNumber.HasValue)
            .Where(a => a.TransmitToGastivo)
            .Where(a => a.PriceCondition == 30)
            .Select(a => new AddressSpecialPricesStruct(a, GetFilteredSpecialPrices(a.ConditionAddress.SpecialPrices)))
            .ToList();

        return deliveryAddressesWithSpecialPrices
                .Concat(billingAddressesWithSpecialPrices)
                .Concat(conditionAddressesWithSpecialPrices)
                .ToList().OrderBy(s => s.Address.AddressNumber);
    }

    private static List<SpecialPrice> GetFilteredSpecialPrices(IEnumerable<SpecialPrice> specialPrices)
    {
        return specialPrices
            .Where(p => p.Article.ArticleNumber.HasValue) // Nur Artikel mit Nummer
            .Where(p => p.Article.Cycle.Id is 1 or 84) // Zyklus "Aktiv" oder "Letzte Fl. eines Jahrgangs"
            .Where(p => p.Article.Filling != null) // Nur mit Abfüllung
            .Where(p => p.Article.DivisionId is null or 1) // Bereich leer oder Casa
            .Where(p => p.Article.ArticleType is >= 2 and <= 5) // Artikeltypen: 2=Wein, 3=Bier, 4=Spirituosen, 5=Mineral & Fruchtsäfte
            .Where(p => p.Article.ECommerceGroup != null) // Nur mit E-Commerce Gruppe
            .Where(p => (!p.ValidFrom.HasValue || DateTime.Now.Date >= p.ValidFrom.Value.Date)
                        && (!p.ValidTo.HasValue || DateTime.Now.Date <= p.ValidTo.Value.Date)) // Gültigkeitsperiode des Preises berücksichtigen
            .ToList();
    }

    private static List<Price> MapToPricesDto(IEnumerable<AddressSpecialPricesStruct> addressSpecialPriceModels)
    {
        var prices = new List<Price>();
        foreach (var addressSpecialPriceModel in addressSpecialPriceModels)
            prices.AddRange(addressSpecialPriceModel.SpecialPrices.Select(specialPrice => new Price
            {
                ArticleNumber = specialPrice.Article.ArticleNumber!.Value.ToString("#"),
                CustomerNumber = addressSpecialPriceModel.Address.AddressNumber!.Value.ToString("#"),
                ListPrice = specialPrice.Price,
                RegularOrderList = specialPrice.Article.StockMovements.Any(s => DateTime.Now <= s.Date.AddDays(360))
                    ? 1
                    : 0
            }));

        return prices;
    }

    private void SerializeAndUpload(IEnumerable<Price> prices)
    {
        var serializedData = GastivoSerializationHelper.SerializeAsCsv(prices, typeof(Price));
        _ftpConnector.UploadFile(serializedData, $"prices-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv");
    }
}

internal record struct AddressSpecialPricesStruct(Address Address, List<SpecialPrice> SpecialPrices)
{
    public static implicit operator (Address a, List<SpecialPrice> SpecialPrices)(AddressSpecialPricesStruct value)
    {
        return (value.Address, value.SpecialPrices);
    }

    public static implicit operator AddressSpecialPricesStruct((Address a, List<SpecialPrice> SpecialPrices) value)
    {
        return new AddressSpecialPricesStruct(value.a, value.SpecialPrices);
    }
}