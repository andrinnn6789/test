using System;
using System.ComponentModel.DataAnnotations;

using IAG.Infrastructure.Crud;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.Base;

public class BaseEntity: IEntityKey<Guid>
{
    public BaseEntity()
    {
        CreatedOn = DateTime.UtcNow;
        ChangedOn = DateTime.UtcNow;
    }
        
    [Key]
    [UsedImplicitly]
    public Guid Id { get; set; }

    [Timestamp]
    [UsedImplicitly]
    public byte[] RowVersion { get; set; }

    [UsedImplicitly]
    public DateTime CreatedOn { get; set; }

    [MaxLength(32)]
    [UsedImplicitly]
    public string CreatedBy { get; set; }

    [UsedImplicitly]
    public DateTime ChangedOn { get; set; }

    [MaxLength(32)]
    [UsedImplicitly]
    public string ChangedBy { get; set; }

    [UsedImplicitly]
    public bool Disabled { get; set; }
}