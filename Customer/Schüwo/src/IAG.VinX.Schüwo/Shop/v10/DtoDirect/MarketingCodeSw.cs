using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Marketing codes, customer extension
/// </summary>
[DataContract]
[DisplayName("MarketingCodeSw")]
[TableCte(@"
        WITH MarketingCodeSw
        AS 
        (
        SELECT 
            Werb_ID                 AS Id, 
            Werb_Bezeichnung        AS Designation,
            Werb_WerbecodeID        AS ParentId,
            ABS(Werb_Online)        AS Online,
            Werb_OnlinePublikation  AS OnlinePublication
        FROM Werbecode
        )
    ")]
public class MarketingCodeSw
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// Designation / Werb_Bezeichnung
    /// </summary>
    [DataMember(Name= "designation")]
    public string Designation { get; set; }

    /// <summary>
    /// Parent id / Werb_WerbecodeID
    /// </summary>
    [DataMember(Name = "parentId")]
    public int? ParentId { get; set; }

    /// <summary>
    /// Online / Werb_Online
    /// </summary>
    [DataMember(Name = "online")]
    public bool Online { get; set; }

    /// <summary>
    /// Online publication / Werb_OnlinePublication
    /// </summary>
    [DataMember(Name = "onlinePublication")]
    public string OnlinePublication { get; set; }
}