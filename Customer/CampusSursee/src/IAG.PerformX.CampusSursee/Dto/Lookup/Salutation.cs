using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Lookup;

[TableCte(@"
       WITH Salutation (
            Id, SalutationNorm, SalutationLetter, Gender, AddressType, 
            Language, LastChange
        ) AS (
        SELECT
        Anr_Id, Anr_Anrede, Anr_Briefanrede, Anr_Geschlecht, Anr_Personentyp,
        CASE Anr_Sprache 
            WHEN 0 THEN 'EN'
            WHEN 1 THEN 'DE'
            WHEN 2 THEN 'FR'
            WHEN 3 THEN 'IT'
        END, 
        Anr_ChangedOn
        FROM Anrede 
        )            
    ")]
[UsedImplicitly]
public class Salutation
{
    public int Id { get; set; }
    public string SalutationNorm { get; set; }
    public string SalutationLetter { get; set; }
    public int Gender { get; set; }
    public int AddressType { get; set; }
    public string Language { get; set; }
    public DateTime LastChange { get; set; }
}