using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class TransferInstallationViewModel
{
    public string ProductName { get; set; }

    public IEnumerable<InstallationViewModel> AvailableSourceInstances { get; set; }

    [Required(ErrorMessage = "Quell-Instanz wird benötigt")]
    public string SourceInstanceName { get; set; }

    public string TargetInstanceName { get; set; }

    public string TargetServiceName { get; set; }

    public string ErrorMessage { get; set; }
        
    public Guid? TransferJobId { get; set; }
}