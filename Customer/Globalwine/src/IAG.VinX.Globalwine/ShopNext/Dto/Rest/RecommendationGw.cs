using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// a single recommendation
/// </summary>
[DataContract]
[DisplayName("Recommendation")]
[TableCte(@"
        WITH RecommendationGw (
            ArticleId, Position, Text
        ) AS (
        SELECT 
            Art_Id, ArtZus_Position, Empf_Bezeichnung
        FROM ArtikelEmpfehlung
        JOIN Empfehlung ON Empf_ID = ArtZus_EmpfehlungID
        JOIN WeinInfo ON Wein_Id = ArtZus_WeininfoID
        JOIN Artikel On Wein_Id = Art_WeininfoID
        WHERE " + ArticleGw.MasterFilter + @"
        )
        ")]
public class RecommendationGw
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
    /// salutation / Empf_Bezeichnung
    /// </summary>
    [DataMember(Name="text")]
    public string Text { get; set; }
}