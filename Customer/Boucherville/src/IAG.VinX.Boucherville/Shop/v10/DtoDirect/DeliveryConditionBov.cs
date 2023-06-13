using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Delivery conditions with customer extensions, Lieferbedingung 
/// </summary>
[DataContract]
[DisplayName("DeliveryConditionBov")]
[TableCte(@"
        WITH DeliveryConditionBov
        AS 
        (
        SELECT 
            Liefbed_ID          AS Id, 
            Liefbed_Bezeichnung AS Designation,

            Liefbed_BezeichnungEN AS DesignationEn
        FROM Lieferbedingung
        WHERE Liefbed_IstOnline=-1 
        )
    ")]
public class DeliveryConditionBov : DeliveryConditionV10
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

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}