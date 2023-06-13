using System;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.ibW.Dto.Azure;

[UsedImplicitly]
[Table("REST3Person")]
public class Person
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public int Gender { get; set; }
    public string EMailP { get; set; }
    public string EMailG { get; set; }
    public string EMailibW { get; set; }
    public string PrinterPin { get; set; }
    public string Phone { get; set; }
    public string MobilePhone { get; set; }
    public string CloudLogin { get; set; }
    public string CloudEMail { get; set; }
    public DateTime? CloudFrom { get; set; }
    public DateTime? CloudTo { get; set; }
    public int CntEmployee { get; set; }
    public int CntInstructor { get; set; }
    public int CntStudent { get; set; }

    [JsonIgnore]
    [Column("IsEmployee")]
    public AtlasBoolean IsEmployeeAtlas { get; set; }
    [NotMapped]
    public bool IsEmployee
    {
        get => IsEmployeeAtlas;
        set => IsEmployeeAtlas = value;
    }
    [JsonIgnore]
    [Column("IsInstructor")]
    public AtlasBoolean IsInstructorAtlas { get; set; }
    [NotMapped]
    public bool IsInstructor
    {
        get => IsInstructorAtlas;
        set => IsInstructorAtlas = value;
    }
    [JsonIgnore]
    [Column("IsStudent")]
    public AtlasBoolean IsStudentAtlas { get; set; }
    [NotMapped]
    public bool IsStudent
    {
        get => IsStudentAtlas;
        set => IsStudentAtlas = value;
    }

    public string Role { get; set; }
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; }
    public string WorkplaceStreet { get; set; }
    public string WorkplaceZip { get; set; }
    public string WorkplaceCity { get; set; }
    public DateTime LastChange { get; set; }
    public string Keywords { get; set; }
}