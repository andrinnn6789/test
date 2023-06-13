using System;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.DdMiddleware.Common.DataAccess;
using IAG.VinX.DdMiddleware.Common.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;
using IAG.VinX.SwissDrink.Resource;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.ProcessEngine;

[UsedImplicitly]
[JobInfo("9EBCFA4D-67E1-4EE4-89E9-F16219201C9C", JobName, true)]
public class ReceiveInvoicesSdlJob : JobBase<ReceiveInvoicesSdlJobConfig, JobParameter, DdJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJobDd + "ReceiveInvoicesSdl";

    private readonly ILogger _logger;
    private readonly ISybaseConnectionFactory _connectionFactory;
    private readonly IInvoiceClient _invoiceClient;
    private readonly IDdInvoiceAccess _invoiceAccess;

    public ReceiveInvoicesSdlJob(
        ILogger<ReceiveInvoicesSdlJob> logger, 
        ISybaseConnectionFactory connectionFactory, 
        IInvoiceClient invoiceClient, 
        IDdInvoiceAccess invoiceAccess)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
        _invoiceClient = invoiceClient;
        _invoiceAccess = invoiceAccess;
    }

    protected override void ExecuteJob()
    {
        var sybaseConnection = _connectionFactory.CreateConnection();
        _invoiceClient.InitClient(
            new RequestResponseLogger(_logger), 
            new SettingsReader(sybaseConnection).GetRestConfig(DataProvider.DigitalDrinkMiddleware)
        );
        var logic = new ReceiveInvoicesSdlLogic(_invoiceAccess, _invoiceClient);
        var jobData = Infrastructure.GetJobData<ReceiveInvoicesSdlJobData>();
        var maxLastCreated = jobData.LastCreatedAtTimestamp ?? DateTime.Today.AddMonths(-1);

        var invoices = logic.ReceiveInvoicesAsync(maxLastCreated).Result;
        foreach (var invoice in invoices)
        {
            try
            {
                AddMessage(MessageTypeEnum.Information, ResourceIds.ProcessInvoiceInfo, invoice.InvoiceNumber);
                if (!logic.ProcessInvoice(invoice, Config.AddressIdZfv))
                    AddMessage(MessageTypeEnum.Information, ResourceIds.InvoiceSkipped, invoice.InvoiceNumber);

                Result.AddedCount++;
                if (invoice.CreatedAt > maxLastCreated)
                    maxLastCreated = invoice.CreatedAt;
            }
            catch (Exception ex)
            {
                AddMessage(ex);
                Result.ErrorCount++;
            }
        }

        jobData.LastCreatedAtTimestamp = maxLastCreated;
        Infrastructure.SetJobData(jobData);
        base.ExecuteJob();
    }
}