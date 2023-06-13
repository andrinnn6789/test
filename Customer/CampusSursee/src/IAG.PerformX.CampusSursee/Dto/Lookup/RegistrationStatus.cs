using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH RegistrationStatus (
            Id, Name, LastChange
        ) AS (
        SELECT
        Status_Id, Status_Bezeichnung, Status_ChangedOn
        FROM Vertragstatus
        WHERE Status_FuerTeilnehmer = -1
        )            
    ")]
[UsedImplicitly]
public class RegistrationStatus
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastChange { get; set; }
}