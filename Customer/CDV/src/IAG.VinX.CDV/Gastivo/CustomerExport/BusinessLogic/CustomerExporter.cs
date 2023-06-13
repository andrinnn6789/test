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
using IAG.VinX.CDV.Gastivo.CustomerExport.Dto;
using IAG.VinX.CDV.Resource;

using NHibernate;

namespace IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;

public class CustomerExporter : ICustomerExporter
{
    private readonly ISessionContextFactory _sessionContextFactory;
    private readonly IFtpConnector _ftpConnector;
    private ISession _session;
    private IMessageLogger _messageLogger;
    
    public CustomerExporter(ISessionContextFactory sessionContextFactory, IFtpConnector ftpConnector)
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

    public GastivoExportJobResult ExportCustomers()
    {
        var result = new GastivoExportJobResult();
        
        try
        {
            var addressModels = GetAddresses();
            var addresses = MapToAddressDto(addressModels);
            SerializeAndUpload(addresses);
            result.ExportedCount = addresses.Count;
        }
        catch (Exception e)
        {
            result.ErrorCount++;
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoCustomerExportError, e);
            ProcessErrorLogger.Log(_messageLogger, _session, ResourceIds.GastivoCustomerExportErrorTitle, string.Format(ResourceIds.GastivoCustomerExportError, e));
        }
        finally
        {
            _session.Dispose();
        }

        result.Result = result.ErrorCount > 0 ? JobResultEnum.Failed : JobResultEnum.Success;

        return result;
    }

    private IEnumerable<Address> GetAddresses()
    {
        return _session.Query<Address>()
            .Where(a => a.IsActive) // Nur aktive Adressen
            .Where(a => a.AddressNumber.HasValue) // Adressnummer nicht leer
            .Where(a => a.TransmitToGastivo) // Übermitteln an Gastivo angekreuzt
            .ToList();
    }

    private static List<Customer> MapToAddressDto(IEnumerable<Address> addressModels)
    {
        return addressModels
            .Select(addressModel =>
                new Customer
                {
                    CustomerNumber = addressModel.AddressNumber!.Value.ToString("#"),
                    Name = !string.IsNullOrEmpty(addressModel.FirstName)
                        ? addressModel.FirstName + " " + addressModel.Name
                        : addressModel.Name,
                    Gln = "",
                    Language = "",
                    DeliveryDayMonday = "X",
                    DeliveryDayTuesday = "X",
                    DeliveryDayWednesday = "X",
                    DeliveryDayThursday = "X",
                    DeliveryDayFriday = "X",
                    DeliveryDaySaturday = "",
                    DeliveryDaySunday = ""
                })
            .ToList();
    }

    private void SerializeAndUpload(IEnumerable<Customer> customers)
    {
        var serializedData = GastivoSerializationHelper.SerializeAsCsv(customers, typeof(Customer));
        _ftpConnector.UploadFile(serializedData, $"customers-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv");
    }
}