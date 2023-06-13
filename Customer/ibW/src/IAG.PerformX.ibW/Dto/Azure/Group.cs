using System;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Dto.Azure;

[UsedImplicitly]
[Table("REST3Group")]
public class Group
{
    public int Id { get; set; }
    public string ShortName { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? ExpirationDateTime { get; set; }
    public DateTime? CloudLoginFrom { get; set; }
    public DateTime? CloudLoginTo { get; set; }

    public bool CreateCloudLogin { get; set; }
    public int SchoolId { get; set; }
    public string EventType { get; set; }
    public string State { get; set; }
    public string SchoolCategory { get; set; }
    public string DepartmentCategory { get; set; }
    public string OfferCategory { get; set; }
    public string Keywords { get; set; }
    public DateTime LastChange { get; set; }
}