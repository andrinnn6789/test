using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class CustomerViewModel
{
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Description { get; set; }
    public string ErrorMessage { get; set; }
    public bool ForceEdit { get; set; }
}