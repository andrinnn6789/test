using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IAG.VinX.Zweifel.S1M.Dto.RequestModels;

public class UploadMediaRequestModel
{
    [Required] public int DocumentNumber { get; set; }
    public IEnumerable<Media> Mediae { get; set; }
}