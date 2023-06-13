using System.ComponentModel.DataAnnotations;

namespace IAG.VinX.Zweifel.S1M.Dto.RequestModels;

public class Media
{
    [Required] public string Content { get; set; }

    [Required] public string Filename { get; set; }
}