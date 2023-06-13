using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.BaseData.Dto.Enums;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Beleg")]
[TablePrefix("Bel")]
[ExcludeFromCodeCoverage]
public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Bel_ID")]
    public int Id { get; set; }

    [Column("Bel_Datum")] public DateTime Date { get; set; }

    [Column("Bel_Logistikstatus")] public LogisticState LogisticState { get; set; }
    
    [Column("Bel_Belegtyp")] public ReceiptType DocumentType { get; set; }
}