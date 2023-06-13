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
public class GoodsIssueLine : IWamasRecord
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

    public string ArticleNumber { get; set; }

    public string Variant { get; set; }

    public string NumQuantityUnit { get; set; }

    public string StockGroup { get; set; }

    public string GoodsOwner { get; set; }

    public string Batch { get; set; }

    public string RemainingLifeValue { get; set; }

    public string RemainingLifeUnit { get; set; }

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

    public string OrderedLuId { get; set; }

    public string OrderedBaseQuantity { get; set; }

    public string ReleasedBaseQuantity { get; set; }

    public string DeliveredBaseQuantity { get; set; }

    public string DeliveredDualQuantity { get; set; }

    public string ReplacementFor { get; set; }

    public string LineType { get; set; }

    public string DeliveryToleranceUpper { get; set; }

    public string DeliveryToleranceLower { get; set; }

    public string RequestedDeliveryDateFrom { get; set; }

    public string RequestedDeliveryDateTo { get; set; }

    public string SapPlant { get; set; }

    public string SapStorageLocation { get; set; }

    public string SapStockType { get; set; }

    public string SapSpecStock { get; set; }

    [FieldHidden] public GoodsIssueLineDifference LineDifference { get; set; }
}