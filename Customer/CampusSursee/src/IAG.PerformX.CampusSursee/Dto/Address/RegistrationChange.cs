using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Address;

[UsedImplicitly]
[Table("Vertrag")]
public class RegistrationChange
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Ver_Id")]
    public int Id { get; set; }
    [Column("Ver_DokumenteWeb")]
    public string WebLinkForUserDocuments { get; set; }
}