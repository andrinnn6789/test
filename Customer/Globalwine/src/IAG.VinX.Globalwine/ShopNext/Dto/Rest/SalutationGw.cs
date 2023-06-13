using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// detail of a salutation
/// </summary>
[DataContract]
[DisplayName("Salutation")]
[Table("Anrede")]
public class SalutationGw
{ 
    /// <summary>
    /// id of the salutation / Anr_ID
    /// </summary>
    [Column("Anr_Id")]
    [DataMember(Name="id")]
    public int? Id { get; set; }

    /// <summary>
    /// salutation / Anr_Anrede
    /// </summary>
    [Column("Anr_Anrede")]
    [DataMember(Name="text")]
    public string Text { get; set; }
}