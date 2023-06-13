using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Address;

[TableCte(
    Consts.RelationSql + @",
        Address (
            Id, AddressTypeName, AddressTypeId, AddressNumber, SocialInsurance, 
            CompanyName, CompanyAdditional, SalutationName, SalutationId, Title, 
            TitleAdd, LastName, LastName2, FirstName, Street,
            Additional1, Additional2, CountryName, CountryId, Zip, 
            Place, Language, NationalityName, NationalityId, DriverLicence, Birthday,
            ReceiveMailings, Profession, HomeTown, SbvIsMember, SbvMemberNumber,
            UserName, PhoneP, PhoneD, PhoneC, HandyP, 
            HandyD, HandyC, EmailP, EmailD, EmailC, EmailDispatchDocuments, CommunicationChannelId,
            Notes, IsHr, LastChange
        ) AS (
        SELECT 
            Adr.Adr_Id, AufArt_Name, AufArt_ID, TRIM(STR(Adr_Adressnummer)), Adr.Adr_VNR,
            Adr.Adr_Firma, Adr.Adr_Firmenzusatz, Anr_Anrede, Anr_Id, Adr.Adr_Titel,
            Adr.Adr_Titelzusatz, Adr.Adr_Name, Adr.Adr_Name2, Adr.Adr_Vorname, Adr.Adr_Strasse, 
            Adr.Adr_Zusatz1, Adr.Adr_Zusatz2, LandAdr.Land_Bezeichnung, LandAdr.Land_Id, Adr.Adr_PLZ,
            Adr.Adr_Ort, Adr.Adr_Sprache, LandNat.Land_Bezeichnung, LandNat.Land_Id, Adr.Adr_FuehrerausweisNummer, Adr.Adr_Geburtsdatum,
            Adr.Adr_ErhaeltMailing, Adr.Adr_Beruf, Adr.Adr_Heimatort, Adr.Adr_SASMitglied, Adr.Adr_SASSBVNummer,
            Adr.Adr_OnlineBenutzername, Adr.Adr_TelP, Adr.Adr_TelD, Adr.Adr_TelG, Adr.Adr_Natel,
            Adr.Adr_NatelD, Adr.Adr_NatelG, Adr.Adr_Email, Adr.Adr_EmailD, Adr.Adr_EmailG, Adr.Adr_EMailDokumentenversand, Adr.Adr_KanalID,
            Adr.Adr_Notizen, IsNull(IsHr, 0),
            GREATER(GREATER(GREATER(Adr.Adr_ChangedOn, Anr_ChangedOn), LandAdr.Land_ChangedOn), IsNull(Relation.LastChange, now() - 9999))
        FROM Adresse Adr
        JOIN Aufnahmeart on Adr.Adr_AufnahmeartID = AufArt_ID
        JOIN Anrede ON Adr.Adr_AnrId = Anr_Id
        JOIN Land LandAdr on LandAdr.Land_Id = Adr.Adr_LandId
        LEFT OUTER JOIN Land LandNat on LandNat.Land_Id = Adr.Adr_NationalitaetId
        LEFT OUTER JOIN Relation ON AddressIdInbound = Adr.Adr_Id
        WHERE " + ExportFilter + @"
        )    
        ")]
[UsedImplicitly]
public class Address
{
    public const string ExportFilter = "Adr.Adr_Aktiv = -1 AND (Adr.Adr_OnlineAktiv = -1 OR Adr.Adr_AufnahmeartID = 1)";

    public int Id { get; set; }
    public string AddressTypeName { get; set; }
    public int AddressTypeId { get; set; }
    public string AddressNumber { get; set; }
    public string SocialInsurance { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAdditional { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string SalutationName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? SalutationId { get; set; }
    public string Title { get; set; }
    public string TitleAdd { get; set; }
    public string LastName { get; set; }
    public string LastName2 { get; set; }
    public string FirstName { get; set; }
    public string Street { get; set; }
    public string Additional1 { get; set; }
    public string Additional2 { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string CountryName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? CountryId { get; set; }
    public string Zip { get; set; }
    public string Place { get; set; }
    public string Language { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string NationalityName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public int? NationalityId { get; set; }
    public string DriverLicence { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public DateTime? Birthday { get; set; }
    public bool ReceiveMailings { get; set; }
    public string Profession { get; set; }
    public string HomeTown { get; set; }
    public int SbvIsMember { get; set; }
    public string SbvMemberNumber { get; set; }
    public string UserName { get; set; }
    public string PhoneP { get; set; }
    public string PhoneD { get; set; }
    public string PhoneC { get; set; }
    public string HandyP { get; set; }
    public string HandyD { get; set; }
    public string HandyC { get; set; }
    public string EmailP { get; set; }
    public string EmailD { get; set; }
    public string EmailC { get; set; }
    public string EmailDispatchDocuments { get; set; }
    public int? CommunicationChannelId { get; set; }
    public string Notes { get; set; }
    public bool IsHr { get; set; }
    public DateTime LastChange { get; set; }
}