using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Customer marketing, customer extension
/// </summary>
[DataContract]
[DisplayName("CustomerMarketingSw")]
[TableCte(@"
        WITH CustomerMarketingSw
        AS 
        (
        SELECT 
            KWerbCode_ID         AS Id, 
            KWerbCode_KundID     AS AddressId,
            KWerbCode_WerbID     AS MarketingId
        FROM Kundenwerbecode
        )
    ")]
public class CustomerMarketingSw
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// FK address / KWerbCode_KundID
    /// </summary>
    [DataMember(Name = "addressId")]
    public int AddressId { get; set; }

    /// <summary>
    /// FK marketing / KWerbCode_WerbID
    /// </summary>
    [DataMember(Name = "marketingId")]
    public int MarketingId { get; set; }
}