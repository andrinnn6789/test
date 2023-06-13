using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// data structure to transmit new addresses and changes
/// </summary>
[DataContract]
[DisplayName("OnlineAddress")]
public class OnlineAddressRestGw
{ 
    /// <summary>
    /// timestamp utc last change, output
    /// </summary>
    [DataMember(Name="changedOn")]
    public DateTime? ChangedOn { get; set; }

    /// <summary>
    /// Structure of the address, input, required.
    /// </summary>
    [DataMember(Name="customerCategory")]
    public AddressstructureGw CustomerCategory { get; set; }

    /// <summary>
    /// id in the shop
    /// </summary>
    [DataMember(Name="shopReference")]
    public string ShopId { get; set; }
    /// <summary>
    /// Gets or Sets ChangeType
    /// </summary>
    [DataMember(Name="changeType")]
    public AddressChangeTypeGw ChangeType { get; set; }

    /// <summary>
    /// id in VinX
    /// </summary>
    [DataMember(Name="id")]
    public int? Id { get; set; }

    /// <summary>
    /// address-id in VinX, only for type [change]
    /// </summary>
    [DataMember(Name="addressId")]
    public int? AddressId { get; set; }

    /// <summary>
    /// title / Adr_Titel
    /// </summary>
    [DataMember(Name="title")]
    public string Title { get; set; }

    /// <summary>
    /// first name / Adr_Vorname
    /// </summary>
    [DataMember(Name="firstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// last name / Adr_Name
    /// </summary>
    [DataMember(Name="lastName")]
    public string LastName { get; set; }

    /// <summary>
    /// name of the company, mapped to lastName if customerCategory = Firma / Adr_Name
    /// </summary>
    [DataMember(Name="company")]
    public string Company { get; set; }

    /// <summary>
    /// email / Adr_Email
    /// </summary>
    [DataMember(Name="email")]
    public string Email { get; set; }

    /// <summary>
    /// homepage / Adr_Homepage
    /// </summary>
    [DataMember(Name="homepage")]
    public string Homepage { get; set; }

    /// <summary>
    /// salutation id (must be known in the shop) / Adr_AnrID
    /// </summary>
    [DataMember(Name="salutationId")]
    public int? SalutationId { get; set; }

    /// <summary>
    /// company phone number / Adr_Natel
    /// </summary>
    [DataMember(Name="phoneNumber")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// additional adress line 1 / Adr_Zusatz1
    /// </summary>
    [DataMember(Name="additionalAddressLine1")]
    public string AdditionalAddressLine1 { get; set; }

    /// <summary>
    /// additional adress line 2 / Adr_Zusatz2
    /// </summary>
    [DataMember(Name="additionalAddressLine2")]
    public string AdditionalAddressLine2 { get; set; }

    /// <summary>
    /// zip code / Adr_Plz
    /// </summary>
    [DataMember(Name="zipcode")]
    public string Zipcode { get; set; }

    /// <summary>
    /// city / Adr_Ort
    /// </summary>
    [DataMember(Name="city")]
    public string City { get; set; }

    /// <summary>
    /// street / Adr_Strasse
    /// </summary>
    [DataMember(Name="street")]
    public string Street { get; set; }

    /// <summary>
    /// country / Land_Code
    /// </summary>
    [DataMember(Name="country")]
    public string Country { get; set; }

    /// <summary>
    /// language-code / Adr_Sprache
    /// </summary>
    public string Language { get; set; } = "DE";

    /// <summary>
    /// country / Adr_LandID
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// login name, must be unique over all addresses and contacts / Adr_OnlineBenutzername
    /// </summary>
    [DataMember(Name="loginName")]
    public string LoginName { get; set; }

    /// <summary>
    /// online active, default = true / Adr_OnlineAktiv
    /// </summary>
    [DataMember(Name="onlineActive")]
    public bool OnlineActive { get; set; } = true;
 
    /// <summary>
    /// Birthday / Adr_Geburtsdatum
    /// </summary>
    [DataMember(Name="birthday")]
    public DateTime? Birthday { get; set; }
}