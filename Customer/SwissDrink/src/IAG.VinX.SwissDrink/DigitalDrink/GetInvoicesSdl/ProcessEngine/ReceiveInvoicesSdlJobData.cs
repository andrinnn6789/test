using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.ProcessEngine;

public class ReceiveInvoicesSdlJobData : IJobData
{
    [ExcludeFromCodeCoverage]
    public Guid Id { get; set; }

    public DateTime? LastCreatedAtTimestamp { get; set; }
}