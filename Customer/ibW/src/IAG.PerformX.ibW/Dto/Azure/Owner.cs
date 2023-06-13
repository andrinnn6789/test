using System;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Dto.Azure;

[UsedImplicitly]
[Table("REST3Owner")]
public class Owner
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int GroupId { get; set; }
    public DateTime LastChange { get; set; }
}