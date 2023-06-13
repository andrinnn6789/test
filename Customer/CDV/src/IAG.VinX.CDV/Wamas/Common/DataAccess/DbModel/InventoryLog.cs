using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("InventoryLog")]
[TablePrefix("Inl")]
[ExcludeFromCodeCoverage]
public class InventoryLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Inl_ID")]
    public int Id { get; set; }

    [Column("Inl_Batch")] public string Batch { get; set; }

    [Column("Inl_MitarbeiterID")] public int UserId { get; set; }

    [Column("Inl_LagerplatzID")] public int? StorageLocationId { get; set; }

    [Column("Inl_ArtikelID")] public int ArticleId { get; set; }

    [Column("Inl_PackageLevel")] public PackageLevel PackageLevel { get; set; }

    [Column("Inl_LevelFactor")] public int LevelFactor { get; set; }

    [Column("Inl_PackageCount")] public int PackageCount { get; set; }

    [Column("Inl_BaseUnitCount")] public int BaseUnitCount { get; set; }

    [Column("Inl_Lot")] public string Lot { get; set; }

    [Column("Inl_Timestamp")] public DateTime Timestamp { get; set; }

    [Column("Inl_ProcessingStatus")] public ProcessingStatus ProcessingStatus { get; set; }

    [Column("Inl_BereichId")] public int AreaId { get; set; }

    [Column("Inl_GUID")] public Guid Guid { get; set; }

    [Column("Inl_ChangedOn")] public DateTime ChangedOn { get; set; }
}