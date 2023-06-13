using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// detail of a rating
/// </summary>
[DataContract]
[DisplayName("Rating")]
[TableCte(@"
        WITH RatingGw (
            ArticleId, Description, Points, RatingYear, RatingVintage, 
            KindId, KindName, KindPointsMin, KindPointsMax, KindUnits
        ) AS (
        SELECT 
            ArtBew_ArtikelID, ArtBew_Text, ArtBew_PunkteText, DatePart(year, ArtBew_Datum), ArtBew_Jahrgang,
            Bewert_ID, Bewert_Bezeichnung, CAST(Bewert_Von AS INTEGER), CAST(Bewert_Bis AS INTEGER), Bewert_Einheit
        FROM ArtikelBewertung
        JOIN Artikel ON Art_Id = ArtBew_ArtikelID
        JOIN Bewertungsart ON Bewert_ID = ArtBew_BewertungsartID
        WHERE " + ArticleGw.MasterFilter + @"
        )
    ")]
public class RatingGw
{ 
    /// <summary>
    /// internal for join operation
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// rating additional text / ArtBew_Text
    /// </summary>
    [DataMember(Name="description")]
    public string Description { get; set; }

    /// <summary>
    /// rating points / ArtBew_Punkte
    /// </summary>
    [DataMember(Name="points")]
    public string Points { get; set; }

    /// <summary>
    /// year of the rating / ArtBew_Datum
    /// </summary>
    [DataMember(Name="ratingYear")]
    public int? RatingYear { get; set; }

    /// <summary>
    /// rated vinatge / ArtBew_Jahrgang
    /// </summary>
    [DataMember(Name="ratingVintage")]
    public int? RatingVintage { get; set; }

    /// <summary>
    /// VinX id of the rating / Bewert_ID
    /// </summary>
    [DataMember(Name="kindId")]
    public int? KindId { get; set; }

    /// <summary>
    /// name of the rating / Bewert_Bezeichnung
    /// </summary>
    [DataMember(Name="kindName")]
    public string KindName { get; set; }

    /// <summary>
    /// minimal points of the rating / Bewert_Von
    /// </summary>
    [DataMember(Name="kindPointsMin")]
    public int? KindPointsMin { get; set; }

    /// <summary>
    /// maximal points of the rating / Bewert_Bis
    /// </summary>
    [DataMember(Name="kindPointsMax")]
    public int? KindPointsMax { get; set; }

    /// <summary>
    /// units of the rating / Bewert_Einheit
    /// </summary>
    [DataMember(Name="kindUnits")]
    public string KindUnits { get; set; }
}