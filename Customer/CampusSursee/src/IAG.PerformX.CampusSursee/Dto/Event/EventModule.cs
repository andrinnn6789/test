using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Event;

[TableCte(@"
      WITH EventModule (
            Id, EventId, ModuleId, Publish, LastChange
        ) AS (
        SELECT
        EreigBez_Id, ev.VerDef_ID, mo.VerDef_ID,
        CASE ev.VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, EreigBez_ChangedOn
        FROM VertragDef mo 
        JOIN VertragDefBeziehung ON EreigBez_GegenVertragDefID = mo.VerDef_Id
        JOIN VertragDef ev ON ev.VerDef_Id = EreigBez_VertragDefID
        WHERE EreigBez_BeziehungstypID IN (7) AND ev." + Event.ExportFilter + @"
    )
    ")]
[UsedImplicitly]
public class EventModule
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int ModuleId { get; set; }
    public bool Publish { get; set; }
    public DateTime LastChange { get; set; }
}