using System;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.CampusSursee.Dto.Event;

[TableCte(@"
       WITH Additional (
            Id, EventId, AtlasInfo, TypeName, TypeId, 
            AtlasDocumentId, FileName, Publish, LastChange
        ) AS (
        SELECT
            VertragDefZusatz_Id, VertragDefZusatz_VertragDefID, VertragDefZusatz_Text, Typ_Bezeichnung, VertragDefZusatz_ZusatzTypID, 
            CASE IsNull(VertragDefZusatz_Datei, '') WHEN '' THEN NULL ELSE VertragDefZusatz_Id END, VertragDefZusatz_Datei, 
            CASE VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, VertragDefZusatz_ChangedOn
        FROM VertragDefZusatz 
        JOIN VertragDef ON VertragDefZusatz_VertragDefID = VerDef_Id
        JOIN Typ ON Typ_Id = VertragDefZusatz_ZusatzTypID

        UNION

        SELECT
            VertragDefZusatz_Id, verdef.VerDef_ID, VertragDefZusatz_Text, Typ_Bezeichnung, VertragDefZusatz_ZusatzTypID, 
            CASE IsNull(VertragDefZusatz_Datei, '') WHEN '' THEN NULL ELSE VertragDefZusatz_Id END, VertragDefZusatz_Datei, 
            CASE verdef.VerDef_Publizieren WHEN 0 THEN 0 ELSE 1 END, VertragDefZusatz_ChangedOn
        FROM VertragDefZusatz 
        JOIN VertragDef template ON VertragDefZusatz_VertragDefID = template.VerDef_Id
        JOIN VertragDef verdef ON verdef.VerDef_SASEreignisVorlage = template.VerDef_Id
        JOIN Typ ON Typ_Id = VertragDefZusatz_ZusatzTypID
        )        
    ")]
[UsedImplicitly]
public class Additional
{
    public int Id { get; set; }
    public int EventId { get; set; }
        
    [JsonIgnore]
    public string AtlasInfo { get; set; }
    [NotMapped]
    public string Info => RtfCleaner.Clean(AtlasInfo);
        
    public string TypeName { get; set; }
    public int TypeId { get; set; }

    [JsonIgnore]
    public int AtlasDocumentId { get; set; }
    [NotMapped]
    public int? DocumentId
    {
        get
        {
            if (AtlasDocumentId == 0)
                return null;
            return AtlasDocumentId + (int) DocumentOffsetEnum.Additional;
        }
    }

    [JsonIgnore]
    public string FileName { get; set; }
    public bool Publish { get; set; }
    public DateTime LastChange { get; set; }
}