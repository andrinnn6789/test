using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Lagerplatz")]
[TablePrefix("Lagplatz")]
[ExcludeFromCodeCoverage]
public class StorageLocation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Lagplatz_ID")]
    public int Id { get; set; }

    [Column("Lagplatz_LagerID")] public int WarehouseId { get; set; }

    [Column("Lagplatz_ArtikelID")] public int ArticleId { get; set; }

    [Column("Lagplatz_Beschriftung")] public string Description { get; set; }
}