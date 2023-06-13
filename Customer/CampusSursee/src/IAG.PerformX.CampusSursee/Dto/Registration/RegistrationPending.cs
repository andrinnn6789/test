using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Registration;

[TableCte(@"
       WITH Registration (
            Id, AddressId, CompanyAddressId, BillingAddressId, KstCustomer, HrAddressId, 
            EventId, NumberOfAttendees, AdditionalAttendees, Remark, EmailForEventDocuments, 
            EmailForEventDocumentsCc, WebLinkForUserDocuments, OvernightStay, Arrival, WithLunch,
            WithSupper, RegistrationCompanyAddressId, RegistrationBillingAddressId, RegistrationHrAddressId, OnWaitingList
        ) AS (
        SELECT 
            Ver_ID, Ver_AdresseID,  Ver_SASGeschaeftsAdresseID, Ver_SASRechnungsadresseID, Ver_KSTKunde, Ver_AdresseIDHRPerson, 
            TRIM(STR(Ver_VertragDefID)), Ver_Personen, Ver_NamenPersonen, Ver_Bemerkungen, Ver_EMailZiel1, 
            Ver_EMailZiel1, Ver_DokumenteWeb, Ver_CSUebernachtung, Ver_CSAnreise, Ver_CSMahlzeitenMittagessen,
            Ver_CSMahlzeitenAbendessen, Ver_OnlineAdresseID, Ver_SASOnlineGeschaeftsAdresseID, Ver_SASOnlineRechnungsadresseID, Ver_CSaufWarteliste
        FROM OnlineVertrag
        JOIN Adresse ON Adr_Id = Ver_AdresseID
        WHERE " + Address.Address.ExportFilter + @" 
        )        
    ")]
[Table("OnlineVertrag")]
public class RegistrationPending
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Ver_Id")]
    public int Id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_AdresseID")]
    public int? AddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_SASGeschaeftsAdresseID")]
    public int? CompanyAddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_SASRechnungsadresseID")]
    public int? BillingAddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_OnlineAdresseID")]
    public int? RegistrationAddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_SASOnlineGeschaeftsAdresseID")]
    public int? RegistrationCompanyAddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_SASOnlineRechnungsadresseID")]
    public int? RegistrationBillingAddressId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [Column("Ver_KSTKunde")]
    public string KstCustomer { get; set; }
    [Column("Ver_AdresseIDHRPerson")]
    public int? HrAddressId { get; set; }
    [Column("Ver_VertragDefID")]
    public int EventId { get; set; }
    [Column("Ver_Personen")]
    public int NumberOfAttendees { get; set; }
    [Column("Ver_NamenPersonen")]
    public string AdditionalAttendees { get; set; }
    [Column("Ver_Bemerkungen")]
    public string Remark { get; set; }
    [Column("Ver_EMailZiel1")]
    public string EmailForEventDocuments { get; set; }
    [Column("Ver_EMailZiel2")]
    public string EmailForEventDocumentsCc { get; set; }
    [Column("Ver_DokumenteWeb")]
    public string WebLinkForUserDocuments { get; set; }
    [Column("Ver_CSUebernachtung")]
    public int OvernightStay { get; set; }
    [Column("Ver_CSAnreise")]
    public int Arrival { get; set; }

    [Column("Ver_CSMahlzeitenMittagessen")]
    [JsonIgnore]
    public AtlasBoolean AtlasWithLunch { get; set; }

    [NotMapped]
    public bool WithLunch
    {
        get => AtlasWithLunch;
        set => AtlasWithLunch = value;
    }

    [Column("Ver_CSMahlzeitenAbendessen")]
    [JsonIgnore]
    public AtlasBoolean AtlasWithSupper { get; set; }

    [NotMapped]
    public bool WithSupper
    {
        get => AtlasWithSupper;
        set => AtlasWithSupper = value;
    }

    [Column("Ver_CSaufWarteliste")]
    [JsonIgnore]
    public AtlasBoolean AtlasOnWaitingList { get; set; }

    [NotMapped]
    public bool OnWaitingList
    {
        get => AtlasOnWaitingList;
        set => AtlasOnWaitingList = value;
    }

    [Column("Ver_Verarbeitet")]
    public bool IsProcessed { get; set; }


    [Column("Ver_VertragID")]
    public int? RegistrationId { get; set; }
}