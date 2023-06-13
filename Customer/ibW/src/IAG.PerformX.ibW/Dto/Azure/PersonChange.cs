using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAG.PerformX.ibW.Dto.Azure;

[Table("Adresse")]
public class PersonChange
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Adr_Id")]
    public int Id { get; set; }
    [Column("Adr_ibWCloudLogin")]
    public string CloudLogin { get; set; }
    [Column("Adr_ibWCloudEMail")]
    public string CloudEMail { get; set; }
}

public class PersonChangeParam
{
    public string CloudLogin { get; set; }
    public string CloudEMail { get; set; }
}