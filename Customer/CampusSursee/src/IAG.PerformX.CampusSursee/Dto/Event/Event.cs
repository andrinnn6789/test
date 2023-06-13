using System;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Event;

[TableCte(@"
        WITH EventBase (
            Id, Number, Title, SubTitle, EventKindName, 
            EventKindId, EventStatusName, EventStatusId, MarketingPush, 
            Publish, PublishFrom, PublishUntil, 
            Language, Place, DateStart, 
            DateEnd, AttendeesMin,
            AttendeesMax, WaitinglistMax, 
            WaitinglistCurrent, 
            AttendeesCurrent,
            ContactMail, ParifondCode, EntranceExamDate, RegistrationFormId, InfoUrl,
            SasExecution,
            LastChange,
            IsExclusive
        ) AS (
        SELECT
            VerDef.VerDef_Id, VerDef.VerDef_Nummer, VerDef.VerDef_Name, VerDef.VerDef_Untertitel, Kind.Typ_Bezeichnung,
            Kind.Typ_Id, Gruppenstatus_Bezeichnung, Gruppenstatus_Id, CASE IsNull(VerDef.VerDef_MarketingPush, 0) WHEN 0 THEN 0 ELSE 1 END, 
            CASE VerDef.VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, VerDef.VerDef_PublizierenAb, VerDef.VerDef_PublizierenBis,
            Lan.Typ_Bezeichnung, VerDef.VerDef_Durchfuehrungsort, VerDef.VerDef_Beginn + IsNull(VerDef.VerDef_Beginnzeit, CONVERT(TIME, '0', 14)), 
            VerDef.VerDef_Ende + IsNull(VerDef.VerDef_Endezeit, CONVERT(TIME, '0', 14)), VerDef.VerDef_MinTN,
            VerDef.VerDef_MaxTN, VerDef.VerDef_MaxWarteliste, 
            VerDef.VerDef_AnzahlWarteliste + SUM(CASE Ver_CSaufWarteliste WHEN -1 THEN Ver_Personen ELSE 0 END), 
            VerDef.VerDef_AnzahlTeilnehmer + SUM(CASE Ver_CSaufWarteliste WHEN 0 THEN Ver_Personen ELSE 0 END), 
            Mit_Email, Sequenz_Bezeichnung, VerDef.VerDef_Aufnahmepruefung, Template.VerDef_AnmeldeformularID, Template.VerDef_URLInformationenZurAusbildung,
            VerDef.VerDef_SASDurchfuehrung,
            GREATER(VerDef.VerDef_ChangedOn, Template.VerDef_ChangedOn),
            VerDef.VerDef_IstExklusiv
        FROM VertragDef_View VerDef
        LEFT OUTER JOIN VertragDef Template ON VerDef.VerDef_SASEreignisVorlage = Template.VerDef_Id
        LEFT OUTER JOIN Typ Kind ON Kind.Typ_Id = VerDef.VerDef_GebietId
        LEFT OUTER JOIN Typ Lan ON Lan.Typ_Id = VerDef.VerDef_ThemaID
        LEFT OUTER JOIN Gruppenstatus ON Gruppenstatus_Id = VerDef.VerDef_StatusID
        LEFT OUTER JOIN Mitarbeiter ON VerDef.VerDef_Assistent_Beginn = Mit_Id
        LEFT OUTER JOIN OnlineVertrag ON Ver_VertragDefID = VerDef.VerDef_Id AND Ver_VertragId IS NULL
        LEFT OUTER JOIN Sequenz ON VerDef.VerDef_SequenzID = Sequenz_ID
        WHERE VerDef." + ExportFilter + @"
        GROUP BY 
            VerDef.VerDef_Id, VerDef.VerDef_Nummer, VerDef.VerDef_Name, VerDef.VerDef_Untertitel, Kind.Typ_Bezeichnung,
            Kind.Typ_Id, Gruppenstatus_Bezeichnung, Gruppenstatus_Id, CASE IsNull(VerDef.VerDef_MarketingPush, 0) WHEN 0 THEN 0 ELSE 1 END, 
            CASE VerDef.VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, VerDef.VerDef_PublizierenAb, VerDef.VerDef_PublizierenBis,
            Lan.Typ_Bezeichnung, VerDef.VerDef_Durchfuehrungsort, VerDef.VerDef_Beginn + IsNull(VerDef.VerDef_Beginnzeit, CONVERT(TIME, '0', 14)), 
            VerDef.VerDef_Ende + IsNull(VerDef.VerDef_Endezeit, CONVERT(TIME, '0', 14)), VerDef.VerDef_MinTN,
            VerDef.VerDef_MaxTN, VerDef.VerDef_MaxWarteliste, 
            VerDef.VerDef_AnzahlWarteliste, 
            VerDef.VerDef_AnzahlTeilnehmer, 
            Mit_Email, Sequenz_Bezeichnung, VerDef.VerDef_Aufnahmepruefung, Template.VerDef_AnmeldeformularID, Template.VerDef_URLInformationenZurAusbildung,
            VerDef.VerDef_SASDurchfuehrung,
            GREATER(VerDef.VerDef_ChangedOn, Template.VerDef_ChangedOn),
            VerDef.VerDef_IstExklusiv
        ),
        AdditionalEvent (
            EventId, AtlasInfo, TypeId, LastChange
        ) AS (
        SELECT
            VertragDefZusatz_VertragDefID, VertragDefZusatz_Text, VertragDefZusatz_ZusatzTypID, 
            VertragDefZusatz_ChangedOn
        FROM VertragDefZusatz 
        JOIN VertragDef ON VertragDefZusatz_VertragDefID = VerDef_Id
        WHERE " + ExportFilter + @"
        ),
        AdditionalTemplate (
            EventId, AtlasInfo, TypeId, 
            AtlasDocumentId, FileName, LastChange
        ) AS (
        SELECT
            verdef.VerDef_ID, VertragDefZusatz_Text, VertragDefZusatz_ZusatzTypID, 
            CASE IsNull(VertragDefZusatz_Datei, '') WHEN '' THEN NULL ELSE VertragDefZusatz_Id END, VertragDefZusatz_Datei, 
            VertragDefZusatz_ChangedOn
        FROM VertragDefZusatz 
        JOIN VertragDef template ON VertragDefZusatz_VertragDefID = template.VerDef_Id
        JOIN VertragDef verdef ON verdef.VerDef_SASEreignisVorlage = template.VerDef_Id
        WHERE verdef." + ExportFilter + @"
        ),
        Category (
            EventId, CategoryName, LastChange
        ) AS (
        SELECT
            verdef.VerDef_ID, List(level1.ECG_Bezeichnung + '>' + level2.ECG_Bezeichnung, ';'), 
            GREATER(GREATER(Max(ECGZV_ChangedOn), MAX(level1.ECG_ChangedOn)), MAX(level1.ECG_ChangedOn))
        FROM ECommerceGruppeZuVertragDef 
        JOIN ECommerceGruppe level2 ON level2.ECG_Id = ECGZV_ECommerceGruppeID
        JOIN ECommerceGruppe level1 ON level1.ECG_Id = level2.ECG_ECommerceGruppeIDUebergeordnet
        JOIN VertragDef template ON ECGZV_VertragDefID = template.VerDef_Id
        JOIN VertragDef verdef ON verdef.VerDef_SASEreignisVorlage = template.VerDef_Id
        WHERE verdef." + ExportFilter + @"
        GROUP BY verdef.VerDef_ID
        ),
        Event (
            Id, Number, Title, SubTitle, EventKindName, 
            EventKindId, EventStatusName, EventStatusId, MarketingPush, Publish, PublishFrom, PublishUntil, 
            Language, Place, DateStart, DateEnd, AttendeesMin,
            AttendeesMax, WaitinglistMax, WaitinglistCurrent, AttendeesCurrent,
            ContactMail, ParifondCode, EntranceExamDate, EntranceExamInfoAtlas, EventAdditionalPriceAtlas,
            EventAvvPriceAtlas, EventDurationAtlas, EventAdditionalInfoAtlas, EventCategories, RegistrationFormId, 
            InfoUrl, SasExecution,
            LastChange, IsExclusive
        ) AS (
        SELECT
            EventBase.Id, Number, Title, SubTitle, EventKindName, 
            EventKindId, EventStatusName, EventStatusId, MarketingPush, Publish, PublishFrom, PublishUntil, 
            Language, Place, DateStart, DateEnd, AttendeesMin,
            AttendeesMax, WaitinglistMax, WaitinglistCurrent, AttendeesCurrent,
            ContactMail, ParifondCode, EntranceExamDate, AddExam.AtlasInfo, AddPrice.AtlasInfo,
            AddAvv.AtlasInfo, AddDur.AtlasInfo, AddAdd.AtlasInfo, CategoryName, RegistrationFormId, 
            InfoUrl, SasExecution,
            GREATER(
                GREATER(
                    GREATER(
                        GREATER(
                            GREATER(
                                GREATER(
                                    EventBase.LastChange, 
                                    Category.LastChange),
                                isnull(AddExam.LastChange, now()-300)), 
                            isnull(AddPrice.LastChange, now()-300)), 
                        isnull(AddAvv.LastChange, now()-300)), 
                    isnull(AddDur.LastChange, now()-300)), 
                isnull(AddAdd.LastChange, now()-300)), 
            EventBase.IsExclusive
        FROM EventBase
        LEFT OUTER JOIN AdditionalEvent AddExam ON EventBase.Id = AddExam.EventId AND AddExam.TypeId = 251
        LEFT OUTER JOIN AdditionalEvent AddAdd ON EventBase.Id = AddAdd.EventId AND AddAdd.TypeId = 252
        LEFT OUTER JOIN AdditionalTemplate AddPrice ON EventBase.Id = AddPrice.EventId AND AddPrice.TypeId = 211
        LEFT OUTER JOIN AdditionalTemplate AddAvv ON EventBase.Id = AddAvv.EventId AND AddAvv.TypeId = 212
        LEFT OUTER JOIN AdditionalTemplate AddDur ON EventBase.Id = AddDur.EventId AND AddDur.TypeId = 213
        LEFT OUTER JOIN Category ON EventBase.Id = Category.EventId
        )
    ")]
[UsedImplicitly]
public class Event
{
    public const string ExportFilter = "VerDef_BereichId = 2";

    public int Id { get; set; }
    public string Number { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public int? RegistrationFormId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string InfoUrl { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string EventKindName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? EventKindId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string EventStatusName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? EventStatusId { get; set; }
    public bool MarketingPush { get; set; }
    public bool Publish { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? PublishFrom { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? PublishUntil { get; set; }
    public string Language { get; set; }
    public string Place { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? DateStart { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? DateEnd { get; set; }
    public int AttendeesMin { get; set; }
    public int AttendeesMax { get; set; }
    public int WaitinglistMax { get; set; }
    public int WaitinglistCurrent { get; set; }
    public int AttendeesCurrent { get; set; }
    public string ContactMail { get; set; }
    public string ParifondCode { get; set; }
    public string SasExecution { get; set; }
    public DateTime? EntranceExamDate { get; set; }

    [JsonIgnore]
    public string EntranceExamInfoAtlas { get; set; }
    [NotMapped]
    public string EntranceExamInfo => RtfCleaner.Clean(EntranceExamInfoAtlas);

    [JsonIgnore]
    public string EventAdditionalPriceAtlas { get; set; }
    [NotMapped]
    public string EventAdditionalPrice => RtfCleaner.Clean(EventAdditionalPriceAtlas);

    [JsonIgnore]
    public string EventAvvPriceAtlas { get; set; }
    [NotMapped]
    public string EventAvvPrice => RtfCleaner.Clean(EventAvvPriceAtlas);

    [JsonIgnore]
    public string EventDurationAtlas { get; set; }
    [NotMapped]
    public string EventDuration => RtfCleaner.Clean(EventDurationAtlas);

    [JsonIgnore]
    public string EventAdditionalInfoAtlas { get; set; }
    [NotMapped]
    public string EventAdditionalInfo => RtfCleaner.Clean(EventAdditionalInfoAtlas);

    public string EventCategories { get; set; }

    public DateTime LastChange { get; set; }

    public bool IsExclusive { get; set; }
}