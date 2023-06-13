using System.Runtime.Serialization;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.Dto;

public enum StockType
{
    [EnumMember(Value = "V")]
    Available,
    [EnumMember(Value = "A")]
    OnRequest
}