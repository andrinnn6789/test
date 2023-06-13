using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH RECURSIVE CatalogRecursive (
        Id, Idp, Sort, Text_De, Level) 
    AS 
    (
        SELECT ArtEGrp_ID, ArtEGrp_ObergruppeID, ArtEGrp_Sort, ArtEGrp_Bezeichnung, 1
        FROM ArtikelEGruppe
        WHERE ArtEGrp_ObergruppeID IS NULL
        UNION ALL
        SELECT CatalogChild.ArtEGrp_ID, CatalogChild.ArtEGrp_ObergruppeID, CatalogChild.ArtEGrp_Sort, CatalogChild.ArtEGrp_Bezeichnung, CatalogRecursive.Level + 1
        FROM ArtikelEGruppe CatalogChild
        JOIN CatalogRecursive ON CatalogChild.ArtEGrp_ObergruppeID = CatalogRecursive.Id
    ),
    CatalogEntry (
        Id, Idp, Sort, Text_De
    )
    AS
    (
        SELECT CAST(Id AS VARCHAR), CAST(Idp AS VARCHAR), CAST(Sort AS INTEGER), Text_De
        FROM CatalogRecursive
        ORDER BY Level
    )
    ")]
[UsedImplicitly]
public class CatalogEntry
{
    public string Id { get; set; }
    public string Idp { get; set; }
    public int Sort { get; set; }
    public string Text_De { get; set; }
}