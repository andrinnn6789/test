using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "artikelstamm")]
public class Articles
{
    [XmlElement("artikel")]
    public List<Article> Items { get; } = new();
}


[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Article
{
    public string artnum { get; set; }

    public string matchcode { get; set; } = string.Empty;

    public string produzent { get; set; } = string.Empty;

    public byte mindbestellung { get; set; }

    public byte status { get; set; }

    public ushort bottlesize { get; set; }

    public string packageunit { get; set; } = string.Empty;

    public string packagetyp { get; set; } = string.Empty;

    public string jahrgang { get; set; } = string.Empty;

    public decimal gewicht { get; set; }

    public string strichcode { get; set; } = string.Empty;

    public string strichcodeverp { get; set; } = string.Empty;

    public byte b2b { get; set; }

    public ushort replacement { get; set; }

    public string bezeichnung1d { get; set; } = string.Empty;

    public string bezeichnung2d { get; set; } = string.Empty;

    public string bezeichnung3d { get; set; } = string.Empty;

    public string landregion { get; set; } = string.Empty;

    public string traubensorte { get; set; } = string.Empty;

    public string traubensorteproz { get; set; } = string.Empty;

    public string herkunft { get; set; } = string.Empty;

    public string vinifikation { get; set; } = string.Empty;

    public string charakteristik { get; set; } = string.Empty;

    public string passendzu { get; set; } = string.Empty;

    public string praemierungen { get; set; } = string.Empty;

    public string bild { get; set; } = string.Empty;

    public byte symvegan { get; set; }

    public byte symleichterwein { get; set; }

    public byte symmittelschwererwein { get; set; }

    public byte symschwererwein { get; set; }

    public byte symfruchtig { get; set; }

    public byte symwuerzig { get; set; }

    public byte symmineralisch { get; set; }

    public byte symbio { get; set; }

    public byte sympaemierung { get; set; }

    public string assortierung { get; set; } = string.Empty;

    public string rabattgruppe { get; set; } = string.Empty;

    public string menustufe1 { get; set; } = string.Empty;

    public string menustufe2 { get; set; } = string.Empty;

    public string menustufe3 { get; set; } = string.Empty;

    public string menustufe4 { get; set; } = string.Empty;

    public string menustufe5 { get; set; } = string.Empty;

    public byte neu { get; set; }

    public string abzugsgruppe { get; set; } = string.Empty;

    [XmlAttribute]
    public uint artvarnum { get; set; }
}