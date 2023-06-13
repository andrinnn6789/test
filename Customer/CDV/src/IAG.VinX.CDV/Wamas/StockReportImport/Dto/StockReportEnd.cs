using System;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.StockReportImport.Dto;

[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class StockReportEnd : IWamasRecord
{
    public string Source { get; set; }

    public string Target { get; set; }
    public int SerialNumber { get; set; }

    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; }

    public string DatasetType { get; set; }

    public string RequestId { get; set; }

    public string PartnerId { get; set; }

    public string WarehouseLocation { get; set; }

    public string ClientId { get; set; }

    public string ItemNumber { get; set; }

    public string Variant { get; set; }

    public string StockGroupId { get; set; }

    public string GoodsOwner { get; set; }

    public string Batch { get; set; }

    public string BestByDateFrom { get; set; }

    public string BestByDateTo { get; set; }

    public string ProductionDateFrom { get; set; }

    public string ProductionDateTo { get; set; }

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

    public string IncludeZeroAmount { get; set; }

    public string GroupByWarehouseLocation { get; set; }

    public string GroupByStorageLocation { get; set; }

    public string GroupByClientId { get; set; }

    public string GroupByLuId { get; set; }

    public string GroupByGoodsOwner { get; set; }

    public string GroupByBatch { get; set; }

    public string GroupByBestByDate { get; set; }

    public string GroupByProductionDate { get; set; }

    public string GroupByReservationNumber { get; set; }

    public string GroupByCrossDockingNumber { get; set; }

    public string GroupByCsia01 { get; set; }

    public string GroupByCsia02 { get; set; }

    public string GroupByCsia03 { get; set; }

    public string GroupByCsia04 { get; set; }

    public string GroupByCsia05 { get; set; }

    public string GroupByCsia06 { get; set; }

    public string GroupByCsia07 { get; set; }

    public string GroupByCsia08 { get; set; }

    public string GroupByCsia09 { get; set; }

    public string GroupByCsia10 { get; set; }

    public string GroupByCsia11 { get; set; }

    public string GroupByCsia12 { get; set; }

    public string GroupByCsia13 { get; set; }

    public string GroupByCsia14 { get; set; }

    public string GroupByCsia15 { get; set; }

    public string GroupByCsia16 { get; set; }

    public string GroupByCsia17 { get; set; }

    public string GroupByCsia18 { get; set; }

    public string GroupByCsia19 { get; set; }

    public string GroupByCsia20 { get; set; }

    public string GroupByInboundDeliveryNumber { get; set; }

    public string GroupByStockGroup { get; set; }

    public string GroupByLuComp { get; set; }

    public string GroupByLuLayout { get; set; }
}