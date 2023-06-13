using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Greiner.EslManager.Dto;

[Serializable]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "article_import")]
public class ArticlesDel
{
    [XmlElement("article")] public List<ArticleDel> Items { get; set; }
}

[TableCte(@"
    WITH ArticleDel (ArtNr, ToDelete)
    AS (
        SELECT Art_Artikelnummer, 1  
        FROM Artikel
        WHERE Art_Aktiv = 0
    )
    ")]
[Serializable]
public class ArticleDel
{
    [XmlAttribute("number")] 
    public int ArtNr { get; set; }

    [XmlAttribute("delete")]
    public bool ToDelete { get; set; }
}