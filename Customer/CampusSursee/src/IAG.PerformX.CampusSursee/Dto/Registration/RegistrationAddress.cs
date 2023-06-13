using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Registration;

public enum AddressChangeTypeEnum
{
    New = 1,
    Change = 2,
    Nop = 3
}

[Table("OnlineAdresse")]
public class RegistrationAddress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Adr_Id")]
    public int Id { get; set; }
    [Column("Adr_Eintragtyp")]
    public int EntryType { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_AdresseID")]
    public int? AddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_AufnahmeartId")]
    public int? AddressTypeId { get; set; }
    [Column("Adr_VNR")]
    public string SocialInsurance { get; set; }
    [Column("Adr_Firma")]
    public string CompanyName { get; set; }
    [Column("Adr_Firmenzusatz")]
    public string CompanyAdditional { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_AnrID")]
    public int? SalutationId { get; set; }
    [Column("Adr_Titel")]
    public string Title { get; set; }
    [Column("Adr_Titelzusatz")]
    public string TitleAdd { get; set; }
    [Column("Adr_Name")]
    public string LastName { get; set; }
    [Column("Adr_Name2")]
    public string LastName2 { get; set; }
    [Column("Adr_Vorname")]
    public string FirstName { get; set; }
    [Column("Adr_Strasse")]
    public string Street { get; set; }
    [Column("Adr_Zusatz1")]
    public string Additional1 { get; set; }
    [Column("Adr_Zusatz2")]
    public string Additional2 { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_LandID")]
    public int? CountryId { get; set; }
    [Column("Adr_PLZ")]
    public string Zip { get; set; }
    [Column("Adr_Ort")]
    public string Place { get; set; }
    [Column("Adr_Sprache")]
    public string Language { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_NationalitaetId")]
    public int? NationalityId { get; set; }
    [Column("Adr_FuehrerausweisNummer")]
    public string DriverLicence { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Adr_Geburtsdatum")]
    public DateTime? Birthday { get; set; }
    [Column("Adr_Beruf")]
    public string Profession { get; set; }
    [Column("Adr_Heimatort")]
    public string HomeTown { get; set; }
    [Column("Adr_OnlineBenutzername")]
    public string UserName { get; set; }
    [Column("Adr_TelP")]
    public string PhoneP { get; set; }
    [Column("Adr_TelD")]
    public string PhoneD { get; set; }
    [Column("Adr_TelG")]
    public string PhoneC { get; set; }
    [Column("Adr_Natel")]
    public string HandyP { get; set; }
    [Column("Adr_NatelD")]
    public string HandyD { get; set; }
    [Column("Adr_NatelG")]
    public string HandyC { get; set; }
    [Column("Adr_Email")]
    public string EmailP { get; set; }
    [Column("Adr_EmailD")]
    public string EmailD { get; set; }
    [Column("Adr_EmailG")]
    public string EmailC { get; set; }
    [Column("Adr_EMailDokumentenversand")]
    public string EmailDispatchDocuments { get; set; }
    [Column("Adr_KanalID")]
    public int? CommunicationChannelId { get; set; }
    [Column("Adr_Notizen")]
    public string Notes { get; set; }
    [Column("Adr_OnlineAktiv")]
    public bool OnlineActive { get; set; } = true;

    [Column("Adr_ErhaeltMailing")]
    [JsonIgnore]
    public AtlasBoolean AtlasReceiveMailings { get; set; }

    [NotMapped]
    public bool ReceiveMailings
    {
        get => AtlasReceiveMailings; 
        set => AtlasReceiveMailings = value;
    }

    [Column("Adr_SASMitglied")]
    public int SbvIsMember { get; set; }

    [Column("Adr_Verarbeitet")]
    [JsonIgnore]
    public AtlasBoolean AtlasProcessed { get; set; }

    [NotMapped]
    public bool Processed
    {
        get => AtlasProcessed;
        set => AtlasProcessed = value;
    }

    [Column("Adr_Foto")]
    [JsonIgnore]
    public byte[] AtlasFoto { get; set; }

    [NotMapped]
    public byte[] Foto
    {
        get => PictureHandler.ExtractPicture(AtlasFoto);
        set => AtlasFoto = PictureHandler.SetPicture(value);
    }

    [NotMapped]
    public AddressChangeTypeEnum ChangeType { get; set; }

    [Column("Adr_isHR")]
    [JsonIgnore]
    public AtlasBoolean AtlasIsHr { get; set; }

    [NotMapped]
    public bool IsHr
    {
        get => AtlasIsHr;
        set => AtlasIsHr = value;
    }
}