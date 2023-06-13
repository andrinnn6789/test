using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

[Table("Bewegung")]
[TablePrefix("Bew")]
[ExcludeFromCodeCoverage]
public class StockMovement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Bew_ID")]
    public int Id { get; set; }

    [Column("Bew_ArtikelId")] public int ArticleId { get; set; }

    [Column("Bew_LagerID")] public int WarehouseId { get; set; }

    [Column("Bew_LagerplatzID")] public int? StorageLocationId { get; set; }

    [Column("Bew_Datum")] public DateTime Date { get; set; }

    [Column("Bew_Vorgang")] public StockMovementType MovementType { get; set; }

    [Column("Bew_Menge")] public decimal Quantity { get; set; }

    [Column("Bew_MengeGG")] public decimal PackagingQuantity { get; set; }

    [Column("Bew_Beschreibung")] public string Description { get; set; }

    [Column("Bew_Anrechenbar")] public int Chargeable { get; set; }

    [Column("Bew_Benutzer")] public string User { get; set; }

    [Column("Bew_DatumMutation")] public DateTime ChangeDate { get; set; }

    [Column("Bew_FuerBestellvorschlag")] public short ForOrderProposal { get; set; }

    [Column("Bew_BereichID")] public int SectionId { get; set; }

    [Column("Bew_MandantID")] public int ClientId { get; set; }
}