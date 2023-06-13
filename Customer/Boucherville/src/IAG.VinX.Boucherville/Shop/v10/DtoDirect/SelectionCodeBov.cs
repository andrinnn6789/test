using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Selection codes with customer extensions / SelektionsCode
/// </summary>
[DataContract]
[DisplayName("SelectionCodeBov")]
[TableCte(@"
        WITH SelectionCodeBov
        AS 
        (
        SELECT 
            Sel_ID                       AS Id, 
            Sel_Bezeichnung              AS Designation,
            Sel_SelektionscodeID         AS ParentId,

            Sel_BezeichnungEN            AS DesignationEn
        FROM SelektionsCode
        )
    ")]
public class SelectionCodeBov : SelectionCodeV10
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