using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedMember.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH Brand AS
    (
        SELECT 
            CAST(Prod_ID AS VARCHAR)    AS Id, 
            Prod_Bezeichnung            AS Name
        FROM Produzent
        WHERE Len(Trim(Prod_Bezeichnung)) > 0
    )
    ")]
[UsedImplicitly]
public class Brand
{
    public string Id { get; set; }
    public string Name { get; set; }
}