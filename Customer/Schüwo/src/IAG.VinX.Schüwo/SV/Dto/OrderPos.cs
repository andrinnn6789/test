using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    OrderPos (
        OrderPosId, OrderPosNr, OrderNr, ExtId, ArtNr, Description, AdrNr, 
        QuantityInBulkPackaging, QuantityInFilling, FillingInCl, FillingTextShort, FillingDescription,
        SellAsUnit, UnitsPerBulkPackaging, BulkPackagingTextShort, BulkPackagingDescription, 
        MainArtEGroupId,
        Price, Vat, OrderStatus, ChangedOn
    )
    AS
    (
        SELECT  
            BelPos_ID, BelPos_Position, Bel_BelegNr, Bel_ExterneID, Art_Artikelnummer, BelPos_Text, Adr_Adressnummer, 
            BelPos_MengeGG, BelPos_MengeAbf, Abf_InhaltInCl, Abf_KuerzelWeb, Abf_Suchbegriff, 
            ABS(Gross_AnbruchErlaubtOnline), Gross_EinhProGG, Gross_KuerzelWeb, Gross_BezeichnungWeb, 
            COALESCE(eGruppeParentParent.ArtEGrp_ID, eGruppeParent.ArtEGrp_ID, eGruppe.ArtEGrp_ID),
            BelPos_Preis * 100, BelPos_MWSTProzent / 100, Bel_Belegstatus, Bel_DatumMutation
        FROM ArtikelPosition
        JOIN Artikel Art ON BelPos_ArtikelID = Art_ID
        JOIN Abfuellung ON Art.Art_AbfID = Abf_ID
        JOIN Grossgebinde ON Art.Art_GrossID = Gross_ID
        LEFT OUTER JOIN ArtikelEGruppe eGruppe ON Art.Art_EGruppeID = eGruppe.ArtEGrp_ID
        LEFT OUTER JOIN ArtikelEGruppe eGruppeParent ON eGruppeParent.ArtEGrp_ID = eGruppe.ArtEGrp_ObergruppeID
        LEFT OUTER JOIN ArtikelEGruppe eGruppeParentParent ON eGruppeParentParent.ArtEGrp_ID = eGruppeParent.ArtEGrp_ObergruppeID
        JOIN Beleg ON BelPos_BelegID = Bel_ID
        JOIN Adresse ON Bel_AdrID = Adr_ID
        WHERE Adr_KKatID = 8 AND Adr_Aktiv = -1 AND Bel_Belegtyp = 30
    )
    ")]
[UsedImplicitly]
public class OrderPos
{
    public int OrderPosId { get; set; }
    public int OrderPosNr { get; set; }
    public int OrderNr { get; set; }
    public string Description { get; set; }
    public int ArtNr { get; set; }
    public int QuantityInBulkPackaging { get; set; }
    public int QuantityInFilling { get; set; }
    public decimal? FillingInCl { get; set; }
    public string FillingTextShort { get; set; }
    public int UnitsPerBulkPackaging { get; set; }
    public string BulkPackagingTextShort { get; set; }
    public bool SellAsUnit { get; set; }
    public int Price { get; set; }
    public decimal Vat { get; set; }
    public int OrderStatus { get; set; }
    public DateTime? ChangedOn { get; set; }
}