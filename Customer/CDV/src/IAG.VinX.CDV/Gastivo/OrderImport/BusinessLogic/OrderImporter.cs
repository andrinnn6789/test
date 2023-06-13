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
using IAG.VinX.CDV.Resource;

using NHibernate;

namespace IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;

public class OrderImporter : IOrderImporter
{
    private readonly ISessionContextFactory _sessionContextFactory;
    private readonly IFtpConnector _ftpConnector;
    private ISession _session;
    private IMessageLogger _messageLogger;

    public OrderImporter(ISessionContextFactory sessionContextFactory, IFtpConnector ftpConnector)
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

    public GastivoImportJobResult ImportOrders()
    {
        var result = new GastivoImportJobResult();

        try
        {
            var filesToImport = _ftpConnector.GetFiles("order");

            foreach (var file in filesToImport)
            {
                ITransaction transaction = null;
                try
                {
                    var onlineOrderDto = DownloadAndDeserialize(file);
                    var onlineOrder = MapToOnlineOrder(onlineOrderDto);

                    transaction = _session.BeginTransaction();
                    _session.Save(onlineOrder);
                    transaction.Commit();

                    _ftpConnector.DeleteFile(file);
                    result.ImportedCount++;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    result.ErrorCount++;
                    _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoOrderImportError,
                        file, e);
                    ProcessErrorLogger.Log(_messageLogger, _session, ResourceIds.GastivoOrderImportErrorTitle, string.Format(ResourceIds.GastivoOrderImportError,
                        file, e));
                }
            }

            if (result.ErrorCount > 0 && result.ImportedCount == 0)
                result.Result = JobResultEnum.Failed;
            else
                result.Result = result.ErrorCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Success;
        }
        catch (Exception e)
        {
            result.Result = JobResultEnum.Failed;
            result.ErrorCount++;

            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoOrdersImportError, e);
            ProcessErrorLogger.Log(_messageLogger, _session, ResourceIds.GastivoOrderImportErrorTitle, string.Format(ResourceIds.GastivoOrdersImportError, e));
        }
        finally
        {
            _session.Dispose();
        }

        return result;
    }
    
    private Dto.OnlineOrder DownloadAndDeserialize(string file)
    {
        var fileData = _ftpConnector.DownloadFile(file);
        var onlineOrder = GastivoSerializationHelper.DeserializeFromXml<Dto.OnlineOrder>(fileData);
        return onlineOrder;
    }
    
    private OnlineOrder MapToOnlineOrder(Dto.OnlineOrder onlineOrderDto)
    {
        var deliveryAddress = _session.Query<Address>().First(a => a.AddressNumber.Value == Convert.ToDecimal(onlineOrderDto.Customer));

        var onlineOrder = new OnlineOrder
        {
            OrderReference = onlineOrderDto.Id,
            OrderDate = onlineOrderDto.OrderDate,
            DeliveryDateRequested = onlineOrderDto.DeliveryDateRequested,
            Hint = onlineOrderDto.Info,
            OrderingAddress = deliveryAddress,
            DeliveryAddress = deliveryAddress,
            ConditionAddress = deliveryAddress,
            DivisionId = 1,
            DeliveryConditionId = deliveryAddress.DeliveryConditionId,
            PaymentConditionId = deliveryAddress.PaymentConditionId,
            IsVatIncluded = 0,
            IsProcessed = 0,
            NumberOfLines = (short?)onlineOrderDto.Articles.Count,
            ProviderId = GetServiceProviderIdByName("Gastivo")
        };

        var onlineOrderLines = new List<OnlineOrderLine>();
        var position = 0;
        
        foreach (var onlineOrderLineDto in onlineOrderDto.Articles)
        {
            var article = _session.Query<Article>().First(a => a.ArticleNumber.Value == Convert.ToDecimal(onlineOrderLineDto.Id));
            var onlineOrderLine = new OnlineOrderLine
            {
                Position = Convert.ToInt16(position),
                Article = article,
                Description = article.Description,
                OrderedQuantity = onlineOrderLineDto.OrderedBaseQuantity,
                UnitPrice = onlineOrderLineDto.UnitPrice,
                Total = onlineOrderLineDto.OrderedBaseQuantity * onlineOrderLineDto.UnitPrice,
                Vat = article.Vat.Percentage,
                Order = onlineOrder
            };
            onlineOrderLines.Add(onlineOrderLine);
            position++;
        }

        onlineOrder.OnlineOrderLines = onlineOrderLines;
        return onlineOrder;
    }   

    private int GetServiceProviderIdByName(string providerDescription) => _session.Query<ServiceProvider>().First(a => a.Description == providerDescription).Id;
}