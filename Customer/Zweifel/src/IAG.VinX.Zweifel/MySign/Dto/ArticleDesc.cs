﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "beschreibungen")]
public class ArticleDescs
{
    [XmlElement("beschreibung")]
    public List<ArticleDesc> Items { get; } = new();
}


[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ArticleDesc
{
    public string text { get; set; } = string.Empty;

    [XmlAttribute]
    public uint artvarnum { get; set; }
}