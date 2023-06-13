using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Artikelposition")]
[TablePrefix("BelPos")]
[ExcludeFromCodeCoverage]
public class ArticlePosition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BelPos_ID")]
    public int Id { get; set; }

    [Column("BelPos_BelegID")] public int DocumentId { get; set; }

    [Column("BelPos_MengeAbf")] public decimal Quantity { get; set; }
    
    [Column("BelPos_Mengezurueckgemeldet")] public decimal QuantityConfirmed { get; set; }
}