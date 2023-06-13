using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Dto.Azure;

[UsedImplicitly]
[Table("REST3AdminUnit")]
public class AdminUnit
{
    public int Id { get; set; }
    public string SchoolName { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string Phone { get; set; }
}