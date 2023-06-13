using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Mitarbeiter")]
[TablePrefix("Mit")]
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Employee
{
    [Key] [Column("Mit_ID")] public int Id { get; set; }
    [Column("Mit_URef")] public string Uref { get; set; }
}