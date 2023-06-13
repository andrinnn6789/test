using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Fehlerprotokoll")]
[TablePrefix("Fehlerprot")]
[ExcludeFromCodeCoverage]
public class ErrorLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Fehlerprot_ID")]
    public int Id { get; set; }

    [Column("Fehlerprot_Bezeichnung")] public string Title { get; set; }
    [Column("Fehlerprot_Aufgetreten")] public string Occurence { get; set; }
    [Column("Fehlerprot_DatumLog")] public DateTime LogDate { get; set; }
    [Column("Fehlerprot_DatumMS")] public int DateMillisecond { get; set; }
    [Column("Fehlerprot_Meldung")] public byte[] Description { get; set; }
    [Column("Fehlerprot_User")] public string User { get; set; }
}