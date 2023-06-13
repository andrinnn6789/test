using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Countries with customer extensions
/// </summary>
[DataContract]
[DisplayName("CountryBov")]
[TableCte(@"
        WITH CountryBov
        AS 
        (
        SELECT 
            Land_ID             AS Id, 
            Land_Bezeichnung    AS Designation,
            Land_Code           AS Code,
            Land_Sort           AS SortOrder,

            Land_BezeichnungEN  AS DesignationEn
        FROM Land
        )
    ")]
public class CountryBov : CountryV10
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