using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schuerch.Shop.v10.DtoDirect;

[DataContract]
[DisplayName("FillingSchuerch")]
[TableCte(@"
WITH FillingSchuerch
        AS 
        (
        SELECT 
            Abf_ID                   AS Id, 
            Abf_BezeichnungWeb       AS Designation,
            Abf_KuerzelWeb           AS ShortNameWeb, 
            Abf_Kuerzel              AS ShortName,
            Abf_Suchbegriff          AS SearchTerm,
            Abf_InhaltInCl           AS ContentInCl,
            ABS(Abf_Verrechenbar)    AS IsChargeable,
            Abf_ArtID                AS ArticleId,
            Abf_Sort                 AS SortOrder,
            CASE Abf_DDRecyclingID
                WHEN 'REC-MW' THEN 'Mehrweg'
                WHEN 'REC-EW' THEN 'Einweg'
            END                      AS DdRecyclingType
        FROM Abfuellung
        )
    ")]
public class FillingSchuerch : FillingV10
{
    /// <summary>
    ///     recycling type, based on Abf_DDRecyclingID
    /// </summary>
    [DataMember(Name = "ddRecyclingType")]
    public string? DdRecyclingType { get; set; }
}