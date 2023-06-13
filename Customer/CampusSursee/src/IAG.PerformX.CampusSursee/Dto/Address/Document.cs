using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Address;

[TableCte(@"
        WITH DocumentRaw (
            Id, AtlasId, DocumentType, DocumentDescription, AddressId, AddressIdParticipant, 
            ParticipantName, EventId, FileName, ArchiveLinkUri, DocumentStatus, 
            DueDate, InvoiceNumber, Receiver, Amount, RowNum, LastChange
        ) AS (
        SELECT DISTINCT
            ArchiveLink_Id + 1100000000, ArchiveLink_Id, 
            CASE Bel_BelegartId 
                WHEN 50 THEN 10
                WHEN 51 THEN 10
                WHEN 60 THEN 20
                ELSE 90
            END, 
            BelegArt_Bezeichnung, Adr.Adr_Id, Adressbeziehung_GegenAdresseID, AdrPart.Adr_Name + ' ' + AdrPart.Adr_VorName,
            Bel_VertragDefID, ArchiveLink_MetaDataTemplate + '.pdf', ArchiveLink_URI, 
            CASE Bel_Status 
                WHEN 30 THEN 1 
                WHEN 40 THEN 3
                WHEN 50 THEN 2
                WHEN 90 THEN 2
            END,
            Bel_ZahlbarBis, Bel_BelegNr, 
            CASE 
                WHEN Bel_AdresseID = Bel_RechnungsAdresseID AND Adr.Adr_AufnahmeartID = 2 THEN 'Privat'
                WHEN Bel_AdresseID <> Bel_RechnungsAdresseID AND Adr.Adr_AufnahmeartID = 1 AND Inv.Adr_AufnahmeartID = 1 THEN 'Separat'
                ELSE 'Firma'
            END,
            Bel_Total, ROW_NUMBER() OVER (PARTITION BY ArchiveLink_ForeignID, ArchiveLink_StdReportID ORDER BY ArchiveLink_ForeignID, ArchiveLink_ID DESC),
            GREATER(ArchiveLink_Druckdatum, Bel_ChangedOn) 
        FROM Adresse Adr
        JOIN Beleg_View ON Bel_AdresseID = Adr.Adr_Id 
        JOIN ArchiveLink ON Bel_Id = ArchiveLink_ForeignID AND ArchiveLink_Tablename = 'Beleg' 
        JOIN SimpleArchive ON Sarc_Id = CAST(SubStr(ArchiveLink_URI, 29) as Int) AND SArc_Content IS NOT NULL
        JOIN Belegart on Bel_BelegartID = BelegArt_ID
        LEFT OUTER JOIN Adresse Inv ON Bel_RechnungsAdresseID = Inv.Adr_ID 
        LEFT OUTER JOIN Adressbeziehung ON Adressbeziehung_Id = Bel_BeziehungID
        LEFT OUTER JOIN Adresse AdrPart ON AdrPart.Adr_Id = Adressbeziehung_GegenAdresseID
        WHERE " + Address.ExportFilter + @" 
            AND Bel_Status IN (30, 40, 50, 90) 
            AND Bel_BelegartId IN (50, 51, 60)

        UNION

        SELECT DISTINCT
            ArchiveLink_Id + 1100000000, ArchiveLink_Id, 50, Mailing_Name, Adr.Adr_Id, NULL, NULL, 
            B_VertragDefID, ArchiveLink_MetaDataTemplate + '.pdf', ArchiveLink_URI, NULL, NULL, 
            NULL, NULL, NULL, ROW_NUMBER() OVER (PARTITION BY Adr.Adr_Id, B_VertragDefID, ArchiveLink_StdReportID ORDER BY ArchiveLink_ID DESC),
            ArchiveLink_Druckdatum 
        FROM Adresse Adr
        JOIN Bewertung ON B_AdresseID = Adr.Adr_Id 
        JOIN ArchiveLink ON B_Id = ArchiveLink_ForeignID AND ArchiveLink_Tablename = 'Bewertung'
        JOIN SimpleArchive ON Sarc_Id = CAST(SubStr(ArchiveLink_URI, 29) as Int) AND SArc_Content IS NOT NULL
        JOIN StdReport ON ArchiveLink_StdReportID = StdRpt_ID
        JOIN Mailing ON Mailing_Id = StdRpt_MailingID
        WHERE " + Address.ExportFilter + @" 
            AND B_Bearbeitungsstatus = 20
            AND Mailing_Publizieren = -1

        UNION

        SELECT DISTINCT
            ArchiveLink_Id + 1100000000, ArchiveLink_Id, 40, Mailing_Name, Adr.Adr_Id, NULL, NULL, 
            Ver_VertragDefID, ArchiveLink_MetaDataTemplate + '.pdf', ArchiveLink_URI, NULL, NULL, 
            NULL, NULL, NULL, 1, ArchiveLink_Druckdatum 
        FROM Adresse Adr
        JOIN Vertrag ON Ver_AdresseID = Adr.Adr_Id 
        JOIN ArchiveLink ON Ver_Id = ArchiveLink_ForeignID AND ArchiveLink_Tablename IN ('Vertrag', 'Mailing')
        JOIN SimpleArchive ON Sarc_Id = CAST(SubStr(ArchiveLink_URI, 29) as Int) AND SArc_Content IS NOT NULL
        JOIN Dokument ON Dokument_Datei = Archivelink_Uri
        JOIN Kontakt ON Kontakt_Id = Dokument_KontaktId
        JOIN Mailing ON Mailing_Id = Kontakt_MailingID
        WHERE " + Address.ExportFilter + @" 
            AND Mailing_Publizieren = -1
        ),
        Document (
            Id, AtlasId, DocumentType, DocumentDescription, AddressId, AddressIdParticipant, 
            ParticipantName, EventId, FileName, ArchiveLinkUri, DocumentStatus, 
            DueDate, InvoiceNumber, Receiver, Amount, LastChange,
            EventStartDate, EventEndDate, EventNumber
        ) AS (
        SELECT 
            Id, AtlasId, DocumentType, DocumentDescription, AddressId, AddressIdParticipant, 
            ParticipantName, EventId, FileName, ArchiveLinkUri, DocumentStatus, 
            DueDate, InvoiceNumber, Receiver, Amount, LastChange,
            VerDef_Beginn + VerDef_Beginnzeit, VerDef_Ende + VerDef_Endezeit, VerDef_Nummer
        FROM DocumentRaw
        LEFT OUTER JOIN VertragDef ON EventId = VerDef_Id
        WHERE RowNum = 1
        )
        ")]
[UsedImplicitly]
public class Document
{
    public int Id { get; set; }
    public int DocumentType { get; set; }
    public string DocumentDescription { get; set; }
    public int AddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? AddressIdParticipant { get; set; }
    public string ParticipantName { get; set; }
    public int? EventId { get; set; }
    public string FileName { get; set; }
    public int DocumentStatus { get; set; }
    public DateTime DueDate { get; set; }
    public int? InvoiceNumber { get; set; }
    public string Receiver { get; set; }
    public decimal? Amount { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string EventNumber { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? EventStartDate { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? EventEndDate { get; set; }
    public DateTime LastChange { get; set; }

    [JsonIgnore]
    public int AtlasId { get; set; }
    [JsonIgnore]
    public string ArchiveLinkUri { get; set; }
}