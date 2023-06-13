using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Article relation, customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleRelationSw")]
[TableCte(@"
        WITH ArticleRelationSw
        AS 
        (
        SELECT 
            Artikelbeziehung_ID             AS Id, 
            Artikelbeziehung_ArtikelID      AS ArticleId1,
            Artikelbeziehung_GegenArtikelID AS ArticleId2
        FROM Artikelbeziehung
        )
    ")]
public class ArticleRelationSw
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name = "id")]
    public int Id { get; set; }

    /// <summary>
    /// FK article 1
    /// </summary>
    [DataMember(Name = "articleId1")]
    public int ArticleId1 { get; set; }

    /// <summary>
    /// FK article 2
    /// </summary>
    [DataMember(Name = "articleId2")]
    public int ArticleId2 { get; set; }
}