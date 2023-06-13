using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public abstract class ChangeInstallationViewModel
{
    public IEnumerable<ReleaseViewModel> AvailableReleases { get; set; }

    [Required(ErrorMessage = "Release wird benötigt")]
    public Guid? SelectedRelease { get; set; }

    public Guid? SelectedConfiguration { get; set; }

    [Required(ErrorMessage = "Instanz wird benötigt")]
    public string SelectedInstance { get; set; }

    public Guid? InstallationJobId { get; set; }

    public string ErrorMessage { get; set; }
}