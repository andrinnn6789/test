using System;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.GoodsIssueImport.Dto;

[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class GoodsIssueLineDifference : IWamasRecord
{
    public string Source { get; set; }

    public string Target { get; set; }

    public int SerialNumber { get; set; }

    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; }

    public string DatasetType { get; set; }

    public string ClientId { get; set; }

    public string Id { get; set; }

    public string PartnerId { get; set; }

    public int LineId { get; set; }

    public int SapLineId { get; set; }

    public string DiffAmountBaseQuantity { get; set; }

    public string DiffCodeId { get; set; }
}