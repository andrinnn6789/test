using System;
using System.Xml.Serialization;

namespace IAG.VinX.Greiner.EslManager.Dto;

[Serializable]
public class TextElement
{
    [XmlAttribute("key")]
    public string Key { get; set; }
    [XmlElement("caption")]
    public string Caption { get; set; }
    [XmlElement("value")]
    public string Value { get; set; }
}