using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Receipt types with customer extensions
/// </summary>
[DataContract]
[DisplayName("ReceiptTypeBov")]
[TableCte(@"
        WITH ReceiptTypeBov
        AS 
        (
        SELECT 
            BelegArt_ID AS Id, 
            BelegArt_Kuerzel AS Designation,
            BelegArt_Bezeichnung AS Description,

            BelegArt_BezeichnungEN AS DescriptionEn
        FROM Belegart
        )
    ")]
public class ReceiptTypeBov : ReceiptTypeV10
{
    [UsedImplicitly]
    public string DescriptionEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DescriptionMultiLang =>
        new()
        {
            { "DE", Description },
            { "EN", DescriptionEn }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}