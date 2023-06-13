using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IAG.VinX.CDV.Gastivo.OrderImport.Dto;

[Serializable]
public class OnlineOrder
{
    public string Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDateRequested { get; set; }
    public string Info { get; set; }

    public int Customer { get; set; }
    
    [XmlArrayItem("Article")]
    public List<OnlineOrderLine> Articles { get; set; }
}