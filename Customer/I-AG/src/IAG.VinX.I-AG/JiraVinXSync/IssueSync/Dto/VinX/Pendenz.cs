using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Pendenz_")]
[Table("Pendenz")]
public class Pendenz
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public string JiraKey { get; set; }

    public string Bezeichnung { get; set; }

    // ReSharper disable once InconsistentNaming
    public int? KostenartID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int? KostenstelleID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int? PendenzID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int? PendenzVerrechnungID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int MitarbeiterID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int ErfasserID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int AdresseID { get; set; }

    public string JiraInfo { get; set; }

    public decimal Aufwand { get; set; }

    public decimal OffertBetrag { get; set; }

    public int Status { get; set; }

    public DateTime? Datum { get; set; }

    public DateTime? Erfassungsdatum { get; set; }

    public DateTime? Ende { get; set; }
}