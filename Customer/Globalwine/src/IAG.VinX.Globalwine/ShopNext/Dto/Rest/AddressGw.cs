using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// details about the addresses
/// </summary>
[DisplayName("Address")]
[DataContract]
public class AddressGw
{ 
    /// <summary>
    /// id in VinX
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// customer number the contact belongs to / Adr_Adressnummer
    /// </summary>
    [DataMember(Name="customerNumber")]
    public int? CustomerNumber { get; set; }

    /// <summary>
    /// id in the shop
    /// </summary>
    [DataMember(Name="shopReference")]
    public string ShopId { get; set; }

    /// <summary>
    /// flags of the types of the address
    /// </summary>
    /// <value>flags of the types of the address</value>
    [DataMember(Name="addresseTypes")]
    public List<AddressTypeGw> AddresseTypes { get; set; }

    /// <summary>
    /// delivery block / !Adr_LieferungErlaubt
    /// </summary>
    [DataMember(Name="deliveryBlock")]
    public bool DeliveryBlock { get; set; }

    /// <summary>
    /// name of the company / Adr_Name
    /// </summary>
    [DataMember(Name="company")]
    public string Company { get; set; }

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
    /// first name / Anr_Anrede
    /// </summary>
    [DataMember(Name="salutationName")]
    public string SalutationName { get; set; }

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
    /// street / Adr_Strasse
    /// </summary>
    [DataMember(Name="street")]
    public string Street { get; set; }

    /// <summary>
    /// city / Adr_Ort
    /// </summary>
    [DataMember(Name="city")]
    public string City { get; set; }

    /// <summary>
    /// country / Land_Code
    /// </summary>
    [DataMember(Name="country")]
    public string Country { get; set; }

    /// <summary>
    /// login name, must be unique over all addresses and contacts / Adr_OnlineBenutzername
    /// </summary>
    [DataMember(Name="loginName")]
    public string LoginName { get; set; }

    /// <summary>
    /// Birthday / Adr_Geburtsdatum
    /// </summary>
    [DataMember(Name="birthday")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Sales right, Adr_Aktionsberechtigung: 10: with, 20 without
    /// </summary>
    [DataMember(Name="salesRight")]
    public int? SalesRight { get; set; }

    /// <summary>
    /// customer category: Privat / Firma
    /// </summary>
    [DataMember(Name="customerCategory")]
    public AddressstructureGw CustomerCategory { get; set; }
}