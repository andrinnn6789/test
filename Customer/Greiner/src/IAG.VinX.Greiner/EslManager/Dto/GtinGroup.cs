using System;
using System.Xml.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Greiner.EslManager.Dto;

[TableCte(@"
    WITH GtinGroup (ArtId, Gtin)
        AS (
            SELECT Art_Id, CAST(Art_EAN1 AS BIGINT)
            FROM Artikel WHERE Art_EAN1 IS NOT NULL
            UNION
            SELECT Art_Id, CAST(Art_EAN2 AS BIGINT)
            FROM Artikel WHERE Art_EAN2 IS NOT NULL
            UNION
            SELECT Art_Id, CAST(Art_EAN3 AS BIGINT)
            FROM Artikel WHERE Art_EAN3 IS NOT NULL
        )
    ")]
[Serializable]
public class GtinGroup
{
    [XmlIgnore]
    public int ArtId { get; set; }
    [XmlElement("gtin")]
    public long Gtin { get; set; }
}