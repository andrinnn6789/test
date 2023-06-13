using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// grape composition part
/// </summary>
[DataContract]
[DisplayName("Composition")]
[TableCte(@"
        WITH CompositionGw (
            ArticleId, Position, GrapeName, Percent
        ) AS (
        SELECT 
            Art_Id, ArtZus_Position, Sorte_Bezeichnung, ArtZus_Anteil
        FROM ArtikelZusammensetzung
        JOIN Traubensorte ON Sorte_ID = ArtZus_TraubensorteID
        JOIN WeinInfo ON Wein_Id = ArtZus_WeininfoID
        JOIN Artikel On Wein_Id = Art_WeininfoID
        WHERE " + ArticleGw.MasterFilter + @"
        )
        ")]
public class CompositionGw
{ 
    /// <summary>
    /// internal for join operation
    /// </summary>
    public int ArticleId { get; set; }
 
    /// <summary>
    /// position in the list / ArtZus_Position
    /// </summary>
    [DataMember(Name="position")]
    public int Position { get; set; }

    /// <summary>
    /// name of the grape / Sorte_Bezeichnung
    /// </summary>
    [DataMember(Name="grapeName")]
    public string GrapeName { get; set; }

    /// <summary>
    /// percent of the grape / ArtZus_Anteil
    /// </summary>
    [DataMember(Name="percent")]
    public decimal? Percent { get; set; }
}