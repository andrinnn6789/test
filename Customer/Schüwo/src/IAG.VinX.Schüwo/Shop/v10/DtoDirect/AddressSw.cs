using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Addresses, table "Adresse", with customer extensions
/// </summary>
[DataContract]
[DisplayName("AddressSw")]
[TableCte(@"
        WITH AddressSw
        AS 
        (
        SELECT 
            Adr_ID                      AS Id, 
            Adr_Adressnummer            AS AddressNumber,
            Adr_Suchbegriff             AS SearchTerm,
            Adr_AnrID                   AS SalutationId,
            Adr_Briefanrede             AS LetterSalutation,
            Adr_Titel                   AS Title,
            Adr_Vorname                 AS FirstName,
            Adr_Name                    AS Name,
            Adr_Firma                   AS CompanyName,
            Adr_Firmenzusatz            AS CompanyAdditionalName,
            Adr_Ort                     AS City,
            Adr_PLZ                     AS ZipCode,
            Adr_Strasse                 AS Street,
            Adr_LandID                  AS CountryId,
            Adr_Zusatz1                 AS AdditionalLine1,
            Adr_Zusatz2                 AS AdditionalLine2,
            Adr_Sprache                 AS Language,
            Adr_Bonusrueckverguetung    AS BonusRevenue,
            Adr_KKatID                  AS CustomerCategoryId,
            Adr_LiefbedID               AS DeliveryConditionId,
            Adr_ZahlkondID              AS PaymentConditionId,
            Adr_RechAdrID               AS BillingAddressId,
            Waehrung_ISO                As Currency,
            Adr_Email                   AS EMail,
            Adr_TelD                    AS PhoneDirect,
            Adr_TelG                    AS PhoneBusiness,
            Adr_TelP                    AS PhonePrivate,
            Adr_FaxG                    AS FaxBusiness,
            Adr_Natel                   AS Mobile,
            Adr_OnlineBenutzername      AS ShopUsername,
            ABS(Adr_Aktiv)              AS IsActive,
            Adr_Geburtsdatum            AS Birthday,
            Adr_KGrpPreisID             AS PriceGroupId,
            Adr_VertrID                 AS SalesRepresentativeId,
            Adr_SortimentID             AS SelectionCodeId,
            CASE Adr_Aktionsberechtigung WHEN 10 THEN 1 ELSE 0 END AS PromotionAllowed,
            Adr_DatumErfassung          AS CreatedDate,
            Adr_DatumMutation           AS ChangedOn,

            Adr_OnlinePasswort          AS ShopUserPwd

        FROM Adresse
        LEFT JOIN Waehrung ON Waehrung_ID=Adr_WaehrungID 
        )
    ")]
public class AddressSw: AddressV10
{
    /// <summary>
    /// Shop user pwd
    /// </summary>
    [DataMember(Name = "shopUserPwd")]
    public string ShopUserPwd { get; set; }
}