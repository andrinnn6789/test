using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH EventKind (
            Id, Name, LastChange
        ) AS (
        SELECT
        Typ_Id, Typ_Bezeichnung, Typ_ChangedOn
        FROM Typ
        WHERE Typ_Verwendung = 50
        )            
    ")]
[UsedImplicitly]
public class EventKind
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastChange { get; set; }
}