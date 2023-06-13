using System;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Dto.Web;

[UsedImplicitly]
[Table("REST3ECommerce")]
public class ECommerce
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Kuerzel { get; set; }
    public string Bezeichnung { get; set; }
    public string BezeichnungIt { get; set; }
    public string Sort { get; set; }
    public DateTime LastChange { get; set; }
}