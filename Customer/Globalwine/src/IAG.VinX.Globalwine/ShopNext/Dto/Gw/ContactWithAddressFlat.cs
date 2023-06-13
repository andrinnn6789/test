using System;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

/// <summary>
/// Internal dto with denormalized data to build contacts and addresses
/// </summary>
[TableCte(@"
        WITH 
        ContactWithAddressFlat AS 
		(
        SELECT 
            KP_Id AS Id, 
            KP_Id AS KpId, 
			ABS(IsNull(KP_ShopAktiv, 1)) * ABS(AdMain.Adr_Aktiv) AS KpActive, 
			KP_AdrID AS KpAddressId,
			KP_ShopID AS KpShopId, 
			KP_Email AS KpEmail, 
			KP_Vorname AS KpFirstName, 
            KP_Name AS KpLastName, 
			KpAnrede.Anr_Anrede AS KpSalutation, 
			KP_Natel AS KpPhoneNumber, 
			KP_OnlineBenutzername AS KpLoginName,
            KP_Geburtsdatum AS KpBirthday,
            AdMain.Adr_Id AS AdId,
			AdMain.Adr_ShopId AS AdShopId, 
			AdMain.Adr_Titel AS AdTitle, 
			AdMain.Adr_Vorname AS AdFirstName, 
            AdMain.Adr_Name AS AdLastName, 
			AdMain.Adr_EMail AS AdEmail, 
			AdMain.Adr_Homepage AS AdHomepage, 
			AdAnrede.Anr_Anrede AS AdSalutation, 
			AdMain.Adr_Natel AS AdPhoneNumber, 
            AdMain.Adr_Zusatz1 AS AdAdditionalAddressLine1, 
			AdMain.Adr_Zusatz2 AS AdAdditionalAddressLine2, 
			AdMain.Adr_Plz AS AdZipcode, 
			AdMain.Adr_Strasse AS AdStreet, 
			AdMain.Adr_Ort AS AdCity,
			AdMain.Adr_OnlineBenutzername AS AdLoginName, 
			AdMain.Adr_Adressnummer AS AdCustomerNumber, 
            AdMain.Adr_Geburtsdatum AS AdBirthday,
            AdMain.Adr_ZahlkondID AS PaymentConditionId,
            LandMain.Land_Code AS AdCountry, 
			ABS(AdMain.Adr_LieferungErlaubt) AS AdDeliveryOk,
            AdMain.Adr_Aktionsberechtigung AS AdSalesRight, 
            KkMain.KundKat_Adressstruktur AS AdCustomerCategory, 
            AdBill.Adr_Id AS BiId, 
			AdBill.Adr_ShopId AS BiShopId, 
			AdBill.Adr_Titel AS BiTitle, 
			AdBill.Adr_Vorname AS BiFirstName, 
            AdBill.Adr_Name AS BiLastName, 
			AdBill.Adr_EMail AS BiEmail, 
			AdBill.Adr_Homepage AS BiHomepage, 
			BiAnrede.Anr_Anrede AS BiSalutation, 
			AdBill.Adr_Natel AS BiPhoneNumber, 
            AdBill.Adr_Zusatz1 AS BiAdditionalAddressLine1, 
			AdBill.Adr_Zusatz2 AS BiAdditionalAddressLine2, 
			AdBill.Adr_Plz AS BiZipcode, 
			AdBill.Adr_Strasse AS BiStreet, 
			AdBill.Adr_Ort AS BiCity,
            LandBill.Land_Code AS BiCountry, 
			AdBill.Adr_OnlineBenutzername AS BiLoginName, 
			AdBill.Adr_Adressnummer AS BiCustomerNumber, 
            AdBill.Adr_Geburtsdatum AS BiBirthday,
            AdBill.Adr_Aktionsberechtigung AS BiSalesRight, 
            KkBill.KundKat_Adressstruktur AS BiCustomerCategory,
            KundPreis_Bezeichnung AS PriceGroupName, 
			KundPreis_ID AS PriceGroupId, 
			KkMain.KundKat_Bezeichnung AS CategoryGroupName, 
			KkMain.KundKat_Id AS CategoryGroupId,            
            GREATER(GREATER(AdMain.Adr_ChangedOn, IsNull(AdBill.Adr_ChangedOn, AdMain.Adr_ChangedOn)), IsNull(KP_ChangedOn, AdMain.Adr_ChangedOn)) AS ChangedOn
        FROM Adresse AdMain
        JOIN KontaktPerson ON KP_AdrID = AdMain.Adr_Id
        LEFT OUTER JOIN Adresse AdBill ON AdBill.Adr_Id = AdMain.Adr_RechAdrID
        LEFT OUTER JOIN KundengruppePreis ON KundPreis_Id = AdMain.Adr_KGrpPreisID
        LEFT OUTER JOIN KundenKategorie KkMain ON KkMain.KundKat_Id = AdMain.Adr_KKatID
        LEFT OUTER JOIN KundenKategorie KkBill ON KkBill.KundKat_Id = AdBill.Adr_KKatID
        LEFT OUTER JOIN Anrede AdAnrede ON AdAnrede.Anr_Id = AdMain.Adr_AnrId
        LEFT OUTER JOIN Anrede BiAnrede ON BiAnrede.Anr_Id = AdBill.Adr_AnrId
        LEFT OUTER JOIN Anrede KpAnrede ON KpAnrede.Anr_Id = KP_AnrId
        LEFT OUTER JOIN Land LandMain ON LandMain.Land_ID = AdMain.Adr_LandId
        LEFT OUTER JOIN Land LandBill ON LandBill.Land_ID = AdBill.Adr_LandId
        WHERE 
            AdMain.Adr_Kunde = -1 AND 
            KP_OnlineBenutzername IS NOT NULL
        )
    ")]
public class ContactWithAddressFlat
{ 
	/// <summary>
	/// timestamp utc last change
	/// </summary>
	public DateTime ChangedOn { get; set; }  

	/// <summary>
	/// id in VinX / KP_Id
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// id in VinX / KP_Id
	/// </summary>
	public int? KpId { get; set; }

	/// <summary>
	/// contact is active or not i.e. can log in / KP_ShopAktiv
	/// </summary>
	public bool KpActive { get; set; }

	/// <summary>
	/// id in VinX of the address the contact belongs to / KP_AdrId
	/// </summary>
	public int KpAddressId { get; set; }

	/// <summary>
	/// id in the shop / KP_ShopID
	/// </summary>
	public string KpShopId { get; set; }

	/// <summary>
	/// first name / KP_Vorname
	/// </summary>
	public string KpFirstName { get; set; }

	/// <summary>
	/// last name / KP_Name
	/// </summary>
	public string KpLastName { get; set; }

	/// <summary>
	/// email / KP_Email
	/// </summary>
	public string KpEmail { get; set; }

	/// <summary>
	/// first name / Anr_Anrede
	/// </summary>
	public string KpSalutation { get; set; }

	/// <summary>
	/// direct phone number or handy number / KP_Tel (fallback KP_Natel)
	/// </summary>
	public string KpPhoneNumber { get; set; }

	/// <summary>
	/// login name, must be unique over all addresses and contacts / KP_OnlineBenutzername
	/// </summary>
	public string KpLoginName { get; set; }
 
	/// <summary>
	/// Birthday / KP_Geburtsdatum
	/// </summary>
	public DateTime? KpBirthday { get; set; }

	/// <summary>
	/// AdMain.Adr_Id
	/// </summary>
	public int AdId { get; set; }

	/// <summary>
	/// Adr_LieferungErlaubt
	/// </summary>
	public bool AdDeliveryOk { get; set; }

	/// <summary>
	/// id in the shop / Adr_ShopID
	/// </summary>
	public string AdShopId { get; set; }
        
	/// <summary>
	/// title / Adr_Titel
	/// </summary>
	public string AdTitle { get; set; }

	/// <summary>
	/// first name / Adr_Vorname
	/// </summary>
	public string AdFirstName { get; set; }

	/// <summary>
	/// last name / Adr_Name
	/// </summary>
	public string AdLastName { get; set; }

	/// <summary>
	/// email / Adr_Email
	/// </summary>
	public string AdEmail { get; set; }

	/// <summary>
	/// homepage / Adr_Homepage
	/// </summary>
	public string AdHomepage { get; set; }

	/// <summary>
	/// first name / Anr_Anrede
	/// </summary>
	public string AdSalutation { get; set; }

	/// <summary>
	/// company phone number / Adr_TelG
	/// </summary>
	public string AdPhoneNumber { get; set; }

	/// <summary>
	/// additional adress line 1 / Adr_Zusatz1
	/// </summary>
	public string AdAdditionalAddressLine1 { get; set; }

	/// <summary>
	/// additional adress line 2 / Adr_Zusatz2
	/// </summary>
	public string AdAdditionalAddressLine2 { get; set; }

	/// <summary>
	/// zip code / Adr_Plz
	/// </summary>
	public string AdZipcode { get; set; }

	/// <summary>
	/// street / Adr_Strasse
	/// </summary>
	public string AdStreet { get; set; }

	/// <summary>
	/// city / Adr_Ort
	/// </summary>
	public string AdCity { get; set; }

	/// <summary>
	/// country / Land_Code
	/// </summary>
	public string AdCountry { get; set; }

	/// <summary>
	/// login name, must be unique over all addresses and contacts / Adr_OnlineBenutzername
	/// </summary>
	public string AdLoginName { get; set; }

	/// <summary>
	/// customer number the contact belongs to / Adr_Adressnummer
	/// </summary>
	public int AdCustomerNumber { get; set; }

	/// <summary>
	/// Sales right, Adr_Aktionsberechtigung: 10: with, 20 without
	/// </summary>
	public int? AdSalesRight { get; set; }
 
	/// <summary>
	/// customer category: Privat / Firma
	/// </summary>
	public AddressstructureGw  AdCustomerCategory { get; set; }
 
	/// <summary>
	/// Birthday / Adr_Geburtsdatum
	/// </summary>
	public DateTime? AdBirthday { get; set; }

	/// <summary>
	/// BillAd.Adr_Id
	/// </summary>
	public int? BiId { get; set; }

	/// <summary>
	/// id in the shop / Adr_ShopID
	/// </summary>
	public string BiShopId { get; set; }

	/// <summary>
	/// title / Adr_Titel
	/// </summary>
	public string BiTitle { get; set; }

	/// <summary>
	/// first name / Adr_Vorname
	/// </summary>
	public string BiFirstName { get; set; }

	/// <summary>
	/// last name / Adr_Name
	/// </summary>
	public string BiLastName { get; set; }

	/// <summary>
	/// email / Adr_Email
	/// </summary>
	public string BiEmail { get; set; }

	/// <summary>
	/// homepage / Adr_Homepage
	/// </summary>
	public string BiHomepage { get; set; }

	/// <summary>
	/// first name / Anr_Anrede
	/// </summary>
	public string BiSalutation { get; set; }

	/// <summary>
	/// company phone number / Adr_TelG
	/// </summary>
	public string BiPhoneNumber { get; set; }

	/// <summary>
	/// additional adress line 1 / Adr_Zusatz1
	/// </summary>
	public string BiAdditionalAddressLine1 { get; set; }

	/// <summary>
	/// additional adress line 2 / Adr_Zusatz2
	/// </summary>
	public string BiAdditionalAddressLine2 { get; set; }

	/// <summary>
	/// zip code / Adr_Plz
	/// </summary>
	public string BiZipcode { get; set; }

	/// <summary>
	/// street / Adr_Strasse
	/// </summary>
	public string BiStreet { get; set; }

	/// <summary>
	/// city / Adr_Ort
	/// </summary>
	public string BiCity { get; set; }

	/// <summary>
	/// country / Land_Code
	/// </summary>
	public string BiCountry { get; set; }

	/// <summary>
	/// login name, must be unique over all addresses and contacts / Adr_OnlineBenutzername
	/// </summary>
	public string BiLoginName { get; set; }

	/// <summary>
	/// customer number the contact belongs to / Adr_Adressnummer
	/// </summary>
	public int? BiCustomerNumber { get; set; }

	/// <summary>
	/// Birthday / Adr_Geburtsdatum
	/// </summary>
	public DateTime? BiBirthday { get; set; }

	/// <summary>
	/// Sales right, Adr_Aktionsberechtigung: 10: with, 20 without
	/// </summary>
	public int? BiSalesRight { get; set; }
 
	/// <summary>
	/// customer category: Privat / Firma
	/// </summary>
	public AddressstructureGw  BiCustomerCategory { get; set; }

	/// <summary>
	/// name of the price group, used to get the right price / Adr_KGrpPreisID - Bezeichnung
	/// </summary>
	public string PriceGroupName { get; set; }

	/// <summary>
	/// id of the price group / Adr_KGrpPreisID
	/// </summary>
	public int? PriceGroupId { get; set; }

	/// <summary>
	/// id of the payment condition / Adr_ZahlkondID
	/// </summary>
	public int? PaymentConditionId { get; set; }

	/// <summary>
	/// name of the category group / Adr_KKatID - Bezeichnung
	/// </summary>
	public string CategoryGroupName { get; set; }

	/// <summary>
	/// id of the price group / Adr_KKatID
	/// </summary>
	public int? CategoryGroupId { get; set; }
}