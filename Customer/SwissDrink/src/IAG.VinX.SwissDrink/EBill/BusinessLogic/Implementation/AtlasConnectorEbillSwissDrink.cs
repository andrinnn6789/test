using IAG.Common.ArchiveProvider;
using IAG.Common.DataLayerSybase;
using IAG.Common.EBill.BusinessLogic.Implementation;

namespace IAG.VinX.SwissDrink.EBill.BusinessLogic.Implementation;

public class AtlasConnectorEbillSwissDrink : AtlasConnectorEbill
{
    public AtlasConnectorEbillSwissDrink(ISybaseConnectionFactory factory, IArchiveProviderFactory archiveProviderFactory) : base(factory, archiveProviderFactory)
    {
    }

    protected override string AtlasOpSql =>
        @"
            SELECT TOP 1
                OP_ID                       AS Id,
                OP_Nummer                   AS Number,
                OP_BelegId                  AS ReceiptId,
                Bel_BelegNr                 AS ReceiptNumber,
                OP_ESRReferenz              AS PaymentReference,
                0.0                         AS PaidAmount,
                OP_Betrag                   AS TotalAmount,
                OP_Datum                    AS IssueDate,
                IsNull(OP_ZahlbarBis, DateAdd(day, Zahlkond_TageNetto, OP_Datum))   
                                            AS DueDate,
                ZahWeg_QRIBAN               AS Iban,
                ZahWeg_BIC                  AS Bic,
                IsNull(Waehrung_ISO, 'CHF') AS CurrencyCode,
                ID_Identifikation           AS EbillRecipientId,
                AdrDel.Adr_Name             AS DelName,
                AdrDel.Adr_Strasse          AS DelStrtNm,
                AdrDel.Adr_PLZ              AS DelPstCd,
                AdrDel.Adr_Ort              AS DelTwnNm,
                'CH'                        AS DelCtry,
                AdrCdtr.Adr_Name            AS CdtrName,
                AdrCdtr.Adr_Strasse         AS CdtrStrtNm,
                AdrCdtr.Adr_PLZ             AS CdtrPstCd,
                AdrCdtr.Adr_Ort             AS CdtrTwnNm,
                'CH'                        AS CdtrCtry,
                RTRIM(RTRIM(CAST(AdrDel.Adr_GLN AS VARCHAR), '0'), '.')    
                                            AS DelGln
            FROM 
                OP
            JOIN Zahlungsweg ON ZahWeg_ID = OP_ZahlungswegID
            LEFT OUTER JOIN Identifikation ON ID_AdresseID = OP_AdresseID AND IsNull(ID_BisDatum, Today()) <= Today()
            LEFT OUTER JOIN Provider ON Provider_ID = ID_ProviderID AND Provider_Typ = 90
            LEFT OUTER JOIN Waehrung ON Waehrung_ID = OP_WaehrungID   
            JOIN Zahlungskondition ON Zahlkond_ID = OP_ZahlungskonditionID
            LEFT OUTER JOIN Beleg ON Bel_Id = Op_BelegID
            LEFT OUTER JOIN Adresse AdrDel ON Bel_LieferAdresseID = AdrDel.Adr_Id
            CROSS JOIN Unternehmen
            JOIN Adresse AdrCdtr ON Unt_AdresseIDEigene = AdrCdtr.Adr_Id
            WHERE OP_ID = ?
            ORDER BY ID_VonDatum DESC
            ";

    protected override string AtlasOpDetailSql =>
        @"                       
            SELECT 
                OPDetail_ID                                                     AS Id,
                OPDetail_OPID                                                   AS OpId,
                IsNull(RTRIM(RTRIM(Cast(IsNull(ArtPos.Art_Artikelnummer, ArtGeb.Art_Artikelnummer) as VARCHAR), '0'), '.'), '-') 
                                                                                AS SellerAssignedId,
                IsNull(IsNull(OPDetail_Text, ArtPos.Art_Bezeichnung), ArtGeb.Art_Bezeichnung)
                                                                                 AS Name,
                CAST(OPDetail_Betrag / (1 + IsNull(OPDetail_MWSTSatz, 0)/100) / OPDetail_Menge AS NUMERIC(20,10)) 
                                                                                AS Price,
                'H87'                                                           AS UnitCode,
                OPDetail_Menge                                                  AS DeliveredQuantity,
                OPDetail_Menge                                                  AS BilledQuantity,
                OPDetail_MWSTSatz                                               AS TaxPercent,
                CASE Adr_MWSTVerrechnung 
                    WHEN 3 THEN 'Export'  
                    WHEN 4 THEN 'Zero'  
                    ELSE CASE IsNull(OPDetail_MWSTSatz, 0) WHEN 0 THEN 'Zero' ELSE 'Obligatory' END
                END                                                             AS TaxCategoryCode,
                CAST(CAST(DDBundle_GTIN AS BIGINT) AS VARCHAR)                  AS Gtin,
                CAST(IsNull(KtoArt.Kto_Nummer, KtoGeb.Kto_Nummer) AS VARCHAR)   AS FinancialAccount
            FROM 
                OPDetail
            JOIN OP ON OP_ID = OPDetail_OpID
            JOIN Adresse ON OP_AdresseID = Adr_ID
            JOIN MWST ON MWST_ID = OPDetail_MWSTID
            LEFT OUTER JOIN ArtikelPosition ON BelPos_Id = OPDetail_ArtikelpositionID
            LEFT OUTER JOIN Artikel ArtPos ON ArtPos.Art_Id = BelPos_ArtikelId
            LEFT OUTER JOIN DDBundle ON ArtPos.Art_DDBundleId = DDBundle_Id
            LEFT OUTER JOIN Artikelkategorie KatArt ON ArtPos.Art_AKatID = KatArt.ArtKat_ID
            LEFT OUTER JOIN Konto KtoArt ON KatArt.ArtKat_KundenKontoID = KtoArt.Kto_ID
            LEFT OUTER JOIN GebindePosition ON GebPos_ID = OPDetail_GebindepositionID
            LEFT OUTER JOIN Artikel ArtGeb ON ArtGeb.Art_Id = GebPos_ArtikelId
            LEFT OUTER JOIN Artikelkategorie KatGeb ON ArtGeb.Art_AKatID = KatGeb.ArtKat_ID
            LEFT OUTER JOIN Konto KtoGeb ON KatGeb.ArtKat_KundenKontoID = KtoGeb.Kto_ID
            WHERE OPDetail_OPID = ?
            ";

    protected override string AtlasVatSql => @"SELECT CAST(Unt_MWSTNr AS VARCHAR) FROM Unternehmen";
}