using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Lager")]
[TablePrefix("Lag")]
[ExcludeFromCodeCoverage]
public class Warehouse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Lag_ID")]
    public int Id { get; set; }

    [Column("Lag_Bezeichnung")] public string Name { get; set; }
}