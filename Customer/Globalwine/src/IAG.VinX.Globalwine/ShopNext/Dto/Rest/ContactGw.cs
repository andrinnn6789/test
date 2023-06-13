using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// details about the contacts. A contact belongs always to one address, but may be related to others for billing and shipping purposes
/// </summary>
[DataContract]
[DisplayName("Contact")]
[Table("KontaktPerson")]
public class ContactGw
{ 
    /// <summary>
    /// id in VinX / KP_Id
    /// </summary>
    [DataMember(Name="id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("KP_Id")]
    public int? Id { get; set; }

    /// <summary>
    /// contact is active or not i.e. can log in / KP_ShopAktiv
    /// </summary>
    [DataMember(Name="active")]
    [Column("KP_ShopAktiv")]
    public bool Active { get; set; }

    /// <summary>
    /// id in VinX of the address the contact belongs to / KP_AdrId
    /// </summary>
    [DataMember(Name="addressId")]
    [Column("KP_AdrId")]
    public int AddressId { get; set; }

    /// <summary>
    /// id in the shop / KP_ShopID
    /// </summary>
    [DataMember(Name="shopReference")]
    [Column("KP_ShopID")]
    public string ShopId { get; set; }

    /// <summary>
    /// timestamp utc last change
    /// </summary>
    [DataMember(Name="changedOn")]
    [Column("KP_ChangedOn")]
    public DateTime ChangedOn { get; set; }

    /// <summary>
    /// first name / KP_Vorname
    /// </summary>
    [DataMember(Name="firstName")]
    [Column("KP_Vorname")]
    public string FirstName { get; set; }

    /// <summary>
    /// last name / KP_Name
    /// </summary>
    [DataMember(Name="lastName")]
    [Column("KP_Name")]
    public string LastName { get; set; }

    /// <summary>
    /// email / KP_Email
    /// </summary>
    [DataMember(Name="email")]
    [Column("KP_Email")]
    public string Email { get; set; }

    /// <summary>
    /// first name / Anr_Anrede
    /// </summary>
    [DataMember(Name="salutationId")]
    [Column("KP_AnrId")]
    public int? SalutationId { get; set; }

    /// <summary>
    /// direct phone number or handy number / KP_Tel
    /// </summary>
    [DataMember(Name="phoneNumer")]
    [Column("KP_Tel")]
    public string PhoneNumer { get; set; }

    /// <summary>
    /// login name, must be unique over all addresses and contacts / KP_OnlineBenutzername
    /// </summary>
    [DataMember(Name="loginName")]
    [Column("KP_OnlineBenutzername")]
    public string LoginName { get; set; }

    /// <summary>
    /// Birthday / KP_Geburtsdatum
    /// </summary>
    [DataMember(Name="birthday")]
    [Column("KP_Geburtsdatum")]
    public string Birthday { get; set; }
}