using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// payment condition, needed for the basket
/// </summary>
[DataContract]
[DisplayName("PaymentCondition")]
[Table("Zahlungskondition")]
public class PaymentConditionGw
{ 
    /// <summary>
    /// id of the condition
    /// </summary>
    [Key]
    [DataMember(Name="id")]
    [Column("Zahlkond_ID")]
    public int? Id { get; set; }

    /// <summary>
    /// short name
    /// </summary>
    [DataMember(Name="name")]
    [Column("Zahlkond_Kurzbezeichnung")]
    public string Name { get; set; }

    /// <summary>
    /// name for end user 
    /// </summary>
    [DataMember(Name="nameShop")]
    [Column("Zahlkond_OnlineBezeichnung")]
    public string NameShop { get; set; }

    /// <summary>
    /// skonto days
    /// </summary>
    [DataMember(Name="skontoDays")]
    [Column("Zahlkond_TageSkonto")]
    public int? SkontoDays { get; set; }

    /// <summary>
    /// skonto percent
    /// </summary>
    [DataMember(Name="skontoPercent")]
    [Column("Zahlkond_ProzentSkonto")]
    public decimal? SkontoPercent { get; set; }

    /// <summary>
    /// payment days
    /// </summary>
    [DataMember(Name="paymentDays")]
    [Column("Zahlkond_TageNetto")]
    public int PaymentDays { get; set; }

    /// <summary>
    /// payment terms
    /// </summary>
    [DataMember(Name="paymentTerms")]
    [Column("Zahlkond_BezeichnungNetto")]
    public string PaymentTerms { get; set; }
}