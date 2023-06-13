using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.Dto;

[Serializable]
[ExcludeFromCodeCoverage]
public class Translation
{
    public string Lang { get; set; }
    public string Value { get; set; }
}