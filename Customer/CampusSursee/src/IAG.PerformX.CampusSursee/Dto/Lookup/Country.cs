using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH Country (
            Id, Name, Code, LastChange
        ) AS (
        SELECT
        Land_Id, Land_Bezeichnung, Land_Code, Land_ChangedOn
        FROM Land
        )            
    ")]
[UsedImplicitly]
public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime LastChange { get; set; }
}