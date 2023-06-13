using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH CommunicationChannel (
            Id, Name, LastChange
        ) AS (
        SELECT
        Kanal_ID, Kanal_Bezeichnung, Kanal_ChangedOn
        FROM Kanal
        )            
    ")]
[UsedImplicitly]
public class CommunicationChannel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastChange { get; set; }
}