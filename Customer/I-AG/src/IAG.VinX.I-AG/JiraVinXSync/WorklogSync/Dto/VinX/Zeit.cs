using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Zeit_")]
[Table("Zeit")]
public class Zeit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }
        
    public TimeSpan? BisZeit { get; set; }
        
    public DateTime Datum { get; set; }

    // ReSharper disable once InconsistentNaming
    public int MitarbeiterID { get; set; }
        
    // ReSharper disable once InconsistentNaming
    public int? PendenzID { get; set; }
        
    public string Problem { get; set; }

    public DateTime? Timestamp { get; set; }
        
    public TimeSpan? VonZeit { get; set; }
        
    public string WorklogId { get; set; }
}