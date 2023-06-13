using System;
using System.IO;
using System.Xml.Serialization;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Resource;
using IAG.VinX.Zweifel.MySign.Dto;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Import;

[JobInfo("5699B770-1E7B-411C-8F9F-DF8A2D731138", JobName)]
public class ImportJob : JobBase<ImportConfig, JobParameter, ImportResult>
{
    public const string JobName = Resource.ResourceIds.ResourcePrefixJob + "MySignImport";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    private int ErrorCount { get; set; }
    private int SuccessCount { get; set; }

    private VinXConnector VinxConnector { get; set; }

    public ImportJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        ErrorCount = 0;
        SuccessCount = 0;
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.ConnectionString);
        using (VinxConnector = new VinXConnector(sybaseConnection))
        {
            Import("*Kunden*.xml", ImportCustomers);
            Import("*Orders*.xml", ImportOrders);
        }

        Result.Result =
            ErrorCount == 0 ? JobResultEnum.Success :
            SuccessCount == 0 ? JobResultEnum.Failed :
            JobResultEnum.PartialSuccess;
        base.ExecuteJob();
    }

    private void Import(string fileMask, Func<string, ImportResultDetail> func)
    {
        foreach (var importFile in Directory.EnumerateFiles(Config.ImportFolder, fileMask))
        {
            var backupFile = Path.GetFileName(importFile);
            try
            {
                var res = func(importFile);
                Result.SyncResult.Add(res);
                if (res.ErrorCount == 0)
                    SuccessCount++;
                else
                    ErrorCount++;
            }
            catch (Exception e)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.GenericError, e.Message);
                AddMessage(MessageTypeEnum.Debug, ResourceIds.GenericError, e);
                ErrorCount++;
            }

            backupFile = Path.Combine(Config.BackupFolder, backupFile + (ErrorCount > 0 ? "Error": "") + DateTime.Now.ToString("yyMMdd-HHmm"));
            if (File.Exists(backupFile))
                File.Delete(backupFile);
            File.Move(importFile, backupFile);

            HeartbeatAndCheckCancellation();
        }
    }

    private ImportResultDetail ImportCustomers(string file)
    {
        var customersShop = ReadXml<Customers>(file);
        var res = new ImportResultDetail {Name = "Customers", RecordCount = customersShop.Items.Count};
        var customersVinX = VinxConnector.GetCustomers();
        foreach (var customerShop in customersShop.Items)
        {
            if (customerShop.idshop == 0)
                continue;
            // new shop-customers have no vinx-id but a shop-id; new vinx-customers have a vinx-id but no shop-id
            // customer-updates have both ids
            var customerVinX = customersVinX.Items.Find(c => c.adrnbr == customerShop.adrnbr) ??
                               customersVinX.Items.Find(c => c.idshop == customerShop.idshop);
            try
            {
                switch (customerShop.Equals(customerVinX))
                {
                    case DiffEnum.Equal:
                    case DiffEnum.IsB2B:
                        break;
                    case DiffEnum.DiffShopId:
                        VinxConnector.SetShopId(customerShop);
                        res.UpdateCount++;
                        break;
                    case DiffEnum.Diff:
                        VinxConnector.UpdateCustomer(customerShop);
                        res.UpdateCount++;
                        break;
                    case DiffEnum.New:
                        VinxConnector.InsertCustomer(customerShop);
                        res.InsertCount++;
                        break;
                }
            }
            catch (Exception e)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.GenericError, e.Message);
                res.ErrorCount++;
            }
            Heartbeat();
        }

        return res;
    }

    private ImportResultDetail ImportOrders(string file)
    {
        var orders = ReadXml<Orders>(file);
        var res = new ImportResultDetail { Name = "Orders", RecordCount = orders.Items.Count};
        foreach (var order in orders.Items)
        {
            try
            {
                order.ShippingCostRef = Config.ShippingCostRef;
                order.PamenyMethodRef = Config.PaymentMethodRef;
                order.PamenyConditionRef = Config.PamenyConditionRef;
                if (VinxConnector.InsertOrder(order))
                    res.InsertCount++;
            }
            catch (Exception e)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.GenericError, e.Message);
                res.ErrorCount++;
            }
            Heartbeat();
        }

        return res;
    }

    private static T ReadXml<T>(string fileName)
    {
        var serializer = new XmlSerializer(typeof(T));
        T data;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            data = (T)serializer.Deserialize(fs);
        }

        return data;
    }
}