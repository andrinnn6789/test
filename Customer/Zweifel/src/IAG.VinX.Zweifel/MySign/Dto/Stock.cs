using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "lagerstamm")]
public class Stocks
{
    [XmlElement("lager")]
    public List<Stock> Items { get; } = new();
}


[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Stock
{
    public int lagernum { get; set; }

    public string lagername { get; set; } = string.Empty;

    public string lagertyp { get; set; } = string.Empty;

    public int menge { get; set; }

    [XmlAttribute]
    public uint artvarnum { get; set; }
}