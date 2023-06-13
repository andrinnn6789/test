using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// details about the contacts. A contact belongs always to one address, but may be related to others for billing and shipping purposes
/// </summary>
[DataContract]
public class ContactWithAddress
{ 
    /// <summary>
    /// id in VinX / KP_Id
    /// </summary>
    [DataMember(Name="id")]
    public int? Id { get; set; }

    /// <summary>
    /// contact is active or not i.e. can log in / KP_ShopAktiv
    /// </summary>
    [DataMember(Name="active")]
    public bool Active { get; set; }

    /// <summary>
    /// id in VinX of the address the contact belongs to / KP_AdrId
    /// </summary>
    [DataMember(Name="addressId")]
    public int AddressId { get; set; }

    /// <summary>
    /// id in the shop / KP_ShopID
    /// </summary>
    [DataMember(Name="shopReference")]
    public string ShopId { get; set; }

    /// <summary>
    /// timestamp utc last change
    /// </summary>
    [DataMember(Name="changedOn")]
    public DateTime ChangedOn { get; set; }

    /// <summary>
    /// name of the company then contact belongs to / Adr_Name
    /// </summary>
    [DataMember(Name="company")]
    public string Company { get; set; }

    /// <summary>
    /// name of the price group, used to get the right price / Adr_KGrpPreisID - Bezeichnung
    /// </summary>
    [DataMember(Name="priceGroupName")]
    public string PriceGroupName { get; set; }

    /// <summary>
    /// id of the price group / Adr_KGrpPreisID
    /// </summary>
    [DataMember(Name="priceGroupId")]
    public int? PriceGroupId { get; set; }

    /// <summary>
    /// name of the category group / Adr_KKatID - Bezeichnung
    /// </summary>
    [DataMember(Name="categoryGroupName")]
    public string CategoryGroupName { get; set; }

    /// <summary>
    /// id of the price group / Adr_KKatID
    /// </summary>
    [DataMember(Name="categoryGroupId")]
    public int? CategoryGroupId { get; set; }
 
    /// <summary>
    /// id of the payment condition / Adr_ZahlkondID
    /// </summary>
    [DataMember(Name="paymentConditionId")]
    public int? PaymentConditionId { get; set; }

    /// <summary>
    /// customer number the contact belongs to / Adr_Adressnummer
    /// </summary>
    [DataMember(Name="customerNumber")]
    public int CustomerNumber { get; set; }

    /// <summary>
    /// first name / KP_Vorname
    /// </summary>
    [DataMember(Name="firstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// last name / KP_Name
    /// </summary>
    [DataMember(Name="lastName")]
    public string LastName { get; set; }

    /// <summary>
    /// email / KP_Email
    /// </summary>
    [DataMember(Name="email")]
    public string Email { get; set; }

    /// <summary>
    /// first name / Anr_Anrede
    /// </summary>
    [DataMember(Name="salutation")]
    public string Salutation { get; set; }

    /// <summary>
    /// handy number / KP_Natel
    /// </summary>
    [DataMember(Name="phoneNumer")]

    public string PhoneNumber { get; set; }

    /// <summary>
    /// Birthday / KP_Geburtsdatum
    /// </summary>
    [DataMember(Name="birthday")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// login name, must be unique over all addresses and contacts / KP_OnlineBenutzername
    /// </summary>
    [DataMember(Name="loginName")]
    public string LoginName { get; set; }

    /// <summary>
    /// list of addresses of a contact
    /// </summary>
    [DataMember(Name="addresses")]
    public List<AddressGw> Addresses { get; set; }
}