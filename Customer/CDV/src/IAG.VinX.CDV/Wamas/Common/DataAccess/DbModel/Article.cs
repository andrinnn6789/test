using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Artikel")]
[TablePrefix("Art")]
[ExcludeFromCodeCoverage]
public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Art_ID")]
    public int Id { get; set; }

    [Column("Art_Artikelnummer")] public decimal ArticleNumber { get; set; }
}