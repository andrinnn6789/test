using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming

namespace IAG.VinX.Greiner.EslManager.Dto;

[Serializable]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "article_import")]
public class Articles
{
    [XmlElement("article")]
    public List<ExportArticle> Items { get; set; }
}
    
    
[Serializable]
public class ExportArticle
{
    [XmlAttribute("number")]
    public int ArtNr { get; set; }
        
    [XmlElement("description")]
    public string Description { get; set; }
        
    [XmlElement("price")]
    public decimal Price { get; set; }
        
    [XmlElement("promotion_price")]
    public decimal? PromotionPrice { get; set; }
        
    [XmlElement("category")]
    public string Category { get; set; }
        
    [XmlElement("article_group")]
    public string ArticleGroup { get; set; }
        
    [XmlElement("deposit")]
    public decimal Deposit { get; set; }
        
    [XmlElement("content")]
    public string Content { get; set; }
        
    [NotMapped]
    [XmlArray("texts")]
    [XmlArrayItem("text")]
    public List<TextElement> TextElements { get; set; }
        
    [NotMapped]
    [XmlArray("gtins")]
    [XmlArrayItem("gtin")]
    public List<long> Gtins { get; set; }
}