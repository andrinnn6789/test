using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH EventStatus (
            Id, Name, LastChange
        ) AS (
        SELECT
        Gruppenstatus_Id, Gruppenstatus_Bezeichnung, Gruppenstatus_ChangedOn
        FROM Gruppenstatus
        WHERE ABS(Gruppenstatus_FuerEreignis) = 1
        )            
    ")]
[UsedImplicitly]
public class EventStatus
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastChange { get; set; }
}