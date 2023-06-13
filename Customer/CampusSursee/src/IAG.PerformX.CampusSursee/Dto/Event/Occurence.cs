using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Event;

[TableCte(@"
       WITH Occurence (
            Id, EventId, DateStart, DateEnd, Publish, LastChange
        ) AS (
        SELECT
        EreigBez_Id, ev.VerDef_ID, te.VerDef_Beginn + te.VerDef_Beginnzeit, te.VerDef_Ende + te.VerDef_Endezeit,
        CASE ev.VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, te.VerDef_ChangedOn
        FROM VertragDef te 
        JOIN VertragDefBeziehung ON EreigBez_GegenVertragDefID = te.VerDef_Id
        JOIN VertragDef ev ON ev.VerDef_Id = EreigBez_VertragDefID
        WHERE EreigBez_BeziehungstypID = 6 AND ev." + Event.ExportFilter + @"
        )            
    ")]
[UsedImplicitly]
public class Occurence
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool Publish { get; set; }
    public DateTime LastChange { get; set; }
}