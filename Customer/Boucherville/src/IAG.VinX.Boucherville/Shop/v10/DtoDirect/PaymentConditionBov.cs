using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// payment conditions with customer extensions, filtered to online active = true
/// </summary>
[DataContract]
[DisplayName("PaymentConditionBov")]
[TableCte(@"
        WITH PaymentConditionBov
        AS 
        (
        SELECT 
            Zahlkond_ID                 AS Id, 
            ISNULL(Zahlkond_OnlineBezeichnung, Zahlkond_Kurzbezeichnung) AS Designation,
            Zahlkond_TageSkonto         AS CashDiscountDays,
            Zahlkond_ProzentSkonto      AS CashDiscountPercentage,
            Zahlkond_TageNetto          AS PaymentDays,
            Zahlkond_BezeichnungNetto   AS PaymentTerms,
            ABS(Zahlkond_Aktiv)         AS Active,


            Zahlkond_OnlineBezeichnungEN AS DesignationEn,
            Zahlkond_BezeichnungNettoEN  AS PaymentTermsEn
        FROM Zahlungskondition
        WHERE Zahlkond_IstOnline = -1
        )
    ")]
public class PaymentConditionBov : PaymentConditionV10
{
    [UsedImplicitly]
    public string DesignationEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DesignationMultiLang =>
        new()
        {
            { "DE", Designation },
            { "EN", DesignationEn }
        };

    [UsedImplicitly]
    public string PaymentTermsEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty PaymentTermsMultiLang =>
        new()
        {
            { "DE", PaymentTerms },
            { "EN", PaymentTermsEn }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}