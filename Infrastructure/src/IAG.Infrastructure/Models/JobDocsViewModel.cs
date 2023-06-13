using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.Models;

[ExcludeFromCodeCoverage]
public class JobDocsViewModel
{
    public List<JobDoc> Docs { get; set; }
    public string ErrorMessage { get; set; }
}