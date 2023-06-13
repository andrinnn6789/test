using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
      WITH AddressRelationKind (
            Id, NameInbound, NameOutbound, LastChange
        ) AS (
        SELECT
        BezIn.BezTyp_ID, BezIn.BezTyp_Bezeichnung, BezOut.BezTyp_Bezeichnung, 
        GREATER(BezIn.BezTyp_ChangedOn, BezOut.BezTyp_ChangedOn)
        FROM Beziehungstyp BezIn 
        JOIN Beziehungstyp BezOut ON BezOut.BezTyp_Id = BezIn.BezTyp_GegentypID
        WHERE BezIn.BezTyp_AufnahmeartID1 = 2
        )    
        ")]
[UsedImplicitly]
public class AddressRelationKind
{
    public int Id { get; set; }
    public string NameInbound { get; set; }
    public string NameOutbound { get; set; }
    public DateTime LastChange { get; set; }
}