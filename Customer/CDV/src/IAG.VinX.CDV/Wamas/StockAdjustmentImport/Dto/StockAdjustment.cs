using System;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.StockAdjustmentImport.Dto;

[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class StockAdjustment : IWamasRecord
{
    public string Source { get; set; }

    public string Target { get; set; }
    public int SerialNumber { get; set; }

    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; }

    public string DatasetType { get; set; }

    public string ClientId { get; set; }

    public string ItemNumber { get; set; }

    public string Variant { get; set; }

    public int NumberOfBaseAndWarehouseQuantityUnit { get; set; }

    public string StockGroupId { get; set; }

    public string GoodsOwner { get; set; }

    public string Batch { get; set; }

    public string BestByDate { get; set; }

    public string ProductionDate { get; set; }

    public string ReservationNumber { get; set; }

    public string CrossDockingNumber { get; set; }

    public string Csia01 { get; set; }

    public string Csia02 { get; set; }

    public string Csia03 { get; set; }

    public string Csia04 { get; set; }

    public string Csia05 { get; set; }

    public string Csia06 { get; set; }

    public string Csia07 { get; set; }

    public string Csia08 { get; set; }

    public string Csia09 { get; set; }

    public string Csia10 { get; set; }

    public string Csia11 { get; set; }

    public string Csia12 { get; set; }

    public string Csia13 { get; set; }

    public string Csia14 { get; set; }

    public string Csia15 { get; set; }

    public string Csia16 { get; set; }

    public string Csia17 { get; set; }

    public string Csia18 { get; set; }

    public string Csia19 { get; set; }

    public string Csia20 { get; set; }

    public string InboudDeliveryNumber { get; set; }

    public string InboundDeliveryLine { get; set; }

    public string LineId { get; set; }

    public string BaseQuantity { get; set; }

    public string DualQuantity { get; set; }

    public string LuId { get; set; }

    public string MasterLuId { get; set; }

    public string WarehouseLocation { get; set; }

    public string StorageLocation { get; set; }

    public string PostingKey { get; set; }

    public string CostCenter { get; set; }

    public string InventoryNumber { get; set; }

    public string ExternalInventoryId { get; set; }
}