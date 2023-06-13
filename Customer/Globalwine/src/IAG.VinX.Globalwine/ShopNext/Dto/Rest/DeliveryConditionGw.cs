using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

[DataContract]
[DisplayName("DeliveryCondition")]
[TableCte(@"
        WITH DeliveryConditionGw AS 
        (
        SELECT 
            Liefbed_Id AS Id,
            Liefbed_Bezeichnung AS Name, 
            ABS(LiefBed_NoShippingCost) AS NoShippingCost
        FROM Lieferbedingung
        WHERE Liefbed_IstOnline = -1
        )
        ")]
public class DeliveryConditionGw
{
    [DataMember(Name="id")]
    public int Id { get; set; }

    [DataMember(Name="name")]
    public string Name { get; set; }

    public bool NoShippingCost{ get; set; }
}