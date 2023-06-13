using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.Models;

[ExcludeFromCodeCoverage]
public class ErrorViewModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
