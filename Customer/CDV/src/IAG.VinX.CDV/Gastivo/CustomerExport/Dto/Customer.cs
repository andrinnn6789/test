using System;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

namespace IAG.VinX.CDV.Gastivo.CustomerExport.Dto;

[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord("\t")]
public class Customer
{
    [FieldCaption("CUSTOMER_NUMBER")]
    public string CustomerNumber { get; set; }
    
    [FieldQuoted]
    [FieldCaption("NAME")]
    public string Name { get; set; }
    
    [FieldCaption("GLN")]
    public string Gln { get; set; }
    
    [FieldCaption("LANGUAGE")]
    public string Language { get; set; }
   
    [FieldCaption("DELIVERY_DAY_MO")]
    public string DeliveryDayMonday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_TU")]
    public string DeliveryDayTuesday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_WE")]
    public string DeliveryDayWednesday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_TH")]
    public string DeliveryDayThursday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_FR")]
    public string DeliveryDayFriday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_SA")]
    public string DeliveryDaySaturday { get; set; }
    
    [FieldCaption("DELIVERY_DAY_SU")]
    public string DeliveryDaySunday { get; set; }
}