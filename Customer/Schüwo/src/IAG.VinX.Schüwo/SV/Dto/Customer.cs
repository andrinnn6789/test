using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    Customer AS
    (
        SELECT
            Adr.Adr_Adressnummer AS AdrNr, 
            Adr.Adr_UID AS Uid, 
            Adr.Adr_Name AS Name, 
            Adr.Adr_Zusatz1 AS AdrAddition1, 
            Adr.Adr_Strasse AS Street, 
            Adr.Adr_PLZ AS Zip, 
            Adr.Adr_Ort AS Town, 
            Adr.Adr_TelG AS TelG, 
            Adr.Adr_FaxG AS FaxG, 
            Land_IsoCode AS CountryIso, 
            Adr.Adr_SVKostenstelle AS SvCostCentreId,
            CASE IsNull(Tour_Montag, 0) WHEN 0 THEN '0' ELSE '1' END +
            CASE IsNull(Tour_Dienstag, 0) WHEN 0 THEN '0' ELSE '1' END +
            CASE IsNull(Tour_Mittwoch, 0) WHEN 0 THEN '0' ELSE '1' END +
            CASE IsNull(Tour_Donnerstag, 0) WHEN 0 THEN '0' ELSE '1' END +
            CASE IsNull(Tour_Freitag, 0) WHEN 0 THEN '0' ELSE '1' END +
            CASE IsNull(Tour_Samstag, 0) WHEN 0 THEN '0' ELSE '1' END + '0' AS Tour,

            CASE IsNull(Tour_Montag, 0) WHEN 0 THEN '' ELSE '5:600' END +
            '|' + CASE IsNull(Tour_Dienstag, 0) WHEN 0 THEN '' ELSE '1:600' END +
            '|' + CASE IsNull(Tour_Mittwoch, 0) WHEN 0 THEN '' ELSE '2:600' END +
            '|' + CASE IsNull(Tour_Donnerstag, 0) WHEN 0 THEN '' ELSE '3:600' END +
            '|' + CASE IsNull(Tour_Freitag, 0) WHEN 0 THEN '' ELSE '4:600' END +
            '|' + CASE IsNull(Tour_Samstag, 0) WHEN 0 THEN '' ELSE '5:600' END + 
            '|' AS Otime
        FROM Adresse Adr
        JOIN Land ON Adr.Adr_LandID = Land_ID
        LEFT OUTER JOIN Tour ON Adr_TourID = Tour_ID
        WHERE Adr_KKatID = 8 AND Adr_Aktiv = -1    )
    ")]
[UsedImplicitly]
public class Customer
{
    public int AdrNr { get; set; }
    public string Uid { get; set; }
    public string Name { get; set; }
    public string AdrAddition1 { get; set; }
    public string Street { get; set; }
    public string Zip { get; set; }
    public string Town { get; set; }
    public string TelG { get; set; }
    public string FaxG { get; set; }
    public string CountryIso { get; set; }
    public string SvCostCentreId { get; set; }
    public string Tour { get; set; }
    public string Otime { get; set; }
}