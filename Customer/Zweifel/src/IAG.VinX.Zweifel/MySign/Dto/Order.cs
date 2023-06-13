using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "Belege")]
public class Orders
{
    [XmlElement("Belegkopf")]
    public List<Order> Items { get; } = new();
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Order
{
    [XmlIgnore]
    public decimal ShippingCostRef { get; set; }

    [XmlIgnore]
    public int PamenyMethodRef { get; set; }

    [XmlIgnore]
    public int PamenyConditionRef { get; set; }

    public int ShopBelegnr { get; set; }

    public int Kundennumer { get; set; }

    public int Belegtyp{ get; set; }

    public string Bestelldatum { get; set; }

    public string Lieferdatum { get; set; }

    public string BelegReferenz { get; set; }

    public string Währung { get; set; }

    public string Kondition { get; set; }

    public decimal saldoBeleg { get; set; }

    public uint liefadr { get; set; }

    [XmlElement("Belegzeile")]
    public List<OrderPos> OrderPos { get; } = new();
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class OrderPos
{
    public string Artikelnummer { get; set; }

    public decimal BestellMenge { get; set; }

    public decimal Liefermenge { get; set; }

    public string Preis { get; set; }

    public string Rabatt { get; set; }

    public string LagerID { get; set; }
}