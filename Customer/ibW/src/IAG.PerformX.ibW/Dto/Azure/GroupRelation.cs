using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Dto.Azure;

[UsedImplicitly]
[Table("REST3GroupRelation")]
public class GroupRelation
{
    public int Id { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public string RelationType { get; set; }
    public int? OppositeRelationId { get; set; }
}