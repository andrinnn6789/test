using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.GoodsIssueImport.Dto;

[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class GoodsIssue : IWamasRecord
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

    public string OrderTime { get; set; }

    public string ShippingMethod { get; set; }

    public string ShippingCategory { get; set; }

    public string Priority { get; set; }

    public string ReleasedWarehouseQuantityUnits { get; set; }

    public string ReleasedTotalDangerousGoodsPoints { get; set; }

    public string ReleasedTotalGrossWeightValue { get; set; }

    public string TotalGrossWeightUnit { get; set; }

    public string ReleasedTotalVolumeValue { get; set; }

    public string ReleasedTotalVolumeUnit { get; set; }

    public string ReleasedNumberLines { get; set; }

    public string ReleasedNumberPrimaryLines { get; set; }

    public string DeliveredNumberWarehouseQuantityUnits { get; set; }

    public string DeliveredTotalDangerousGoodsPoints { get; set; }

    public string DeliveredTotalGrossWeightValue { get; set; }

    public string DeliveredTotalGrossWeightUnit { get; set; }

    public string DeliveredTotalVolumeValue { get; set; }

    public string DeliveredTotalVolumeUnit { get; set; }

    public string MainState { get; set; }

    public string ReleaseState { get; set; }

    public string Canceled { get; set; }

    [FieldHidden] [FieldValueDiscarded] public List<GoodsIssueLine> Lines { get; set; } = new();
}