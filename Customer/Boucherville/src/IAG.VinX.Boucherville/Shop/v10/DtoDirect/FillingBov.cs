using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Fillings of the articles with customer extensions, Abfuellung
/// </summary>
[DataContract]
[DisplayName("FillingBov")]
[TableCte(@"
        WITH FillingBov
        AS 
        (
        SELECT 
            Abf_ID                   AS Id, 
            Abf_BezeichnungWeb       AS Designation,
            Abf_KuerzelWeb           AS ShortNameWeb, 
            Abf_Kuerzel              AS ShortName,
            Abf_Suchbegriff          AS SearchTerm,
            Abf_InhaltInCl           AS ContentInCl,
            ABS(Abf_Verrechenbar)    AS IsChargeable,
            Abf_ArtID                AS ArticleId,
            Abf_Sort                 AS SortOrder,

            Abf_BezeichnungWebEN     AS DesignationEn,
            Abf_KuerzelWebEN         AS ShortNameWebEn,
            Abf_KuerzelEN            AS ShortNameEn
        FROM Abfuellung
        )
    ")]
public class FillingBov : FillingV10
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
    public string ShortNameWebEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty ShortNameWebMultiLang =>
        new()
        {
            { "DE", ShortNameWeb },
            { "EN", ShortNameWebEn }
        };

    [UsedImplicitly]
    public string ShortNameEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty ShortNameMultiLang =>
        new()
        {
            { "DE", ShortName },
            { "EN", ShortNameEn }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}