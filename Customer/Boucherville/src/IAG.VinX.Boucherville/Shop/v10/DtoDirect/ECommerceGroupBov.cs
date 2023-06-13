using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// E-commerce grouping of the articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ECommerceGroupBov")]
[TableCte(@"
        WITH ECommerceGroupBov
        AS 
        (
        SELECT 
            ArtEGrp_ID AS Id, 
            ArtEGrp_Bezeichnung AS Designation,
            ArtEGrp_ObergruppeID AS ParentId,
            ArtEGrp_Sort AS SortKey,

            ArtEGrp_BezeichnungEN AS DesignationEn
        FROM ArtikelEGruppe
        )
    ")]
public class ECommerceGroupBov : ECommerceGroupV10
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