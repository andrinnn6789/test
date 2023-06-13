using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Regions with customer extensions
/// </summary>
[DataContract]
[DisplayName("RegionBov")]
[TableCte(@"
        WITH RegionBov
        AS 
        (
        SELECT 
            Reg_ID              AS Id, 
            Reg_Bezeichnung     AS Designation,
            Reg_LandID          AS CountryId,
            Reg_RegID           AS ParentId,
            Reg_Sort            AS SortOrder,

            Reg_BezeichnungEN   AS DesignationEn
        FROM Region
        )
    ")]
public class RegionBov : RegionV10
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