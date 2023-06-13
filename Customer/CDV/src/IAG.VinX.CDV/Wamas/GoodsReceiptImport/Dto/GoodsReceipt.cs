using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.GoodsReceiptImport.Dto;

[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class GoodsReceipt : IWamasRecord
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

    public string WarehouseLocation { get; set; }

    public string Type { get; set; }

    public string DeliveryTimeSlotFrom { get; set; }

    public string DeliveryTimeSlotTo { get; set; }

    public string DeliveryNoteNumber { get; set; }

    public string DeliveryNoteDate { get; set; }

    public string Priority { get; set; }

    public string MainState { get; set; }

    public string LaExchangeState { get; set; }

    [FieldHidden] [FieldValueDiscarded] public List<GoodsReceiptLine> Lines { get; set; } = new();
}