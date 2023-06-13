using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "preisstamm")]
public class Prices
{
    [XmlElement("artikel")]
    public List<Price> Items { get; } = new();
}


[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Price
{
    public string grpid { get; set; } = "P01";

    public string artgrp { get; set; }

    public uint adrnbr { get; set; } = 0;

    public ushort prstyp { get; set; } = 1;

    public ushort verarbeitungstyp { get; set; } = 3;

    public string gultvon { get; set; } = string.Empty;

    public string gultbis { get; set; } = string.Empty;

    public string whgid { get; set; } = "CHF";

    public ushort prsstflstufe { get; set; } = 0;

    public string prs { get; set; }

    public ushort brunet { get; set; } = 1;

    public string satz { get; set; }

    [XmlAttribute]
    public uint artvarnum;
}