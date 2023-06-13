using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.Models;

[ExcludeFromCodeCoverage]
public class JobDoc
{
    public Guid TemplateId { get; set; }
    public string JobName { get; set; }
    public bool IsJobActive { get; set; }
    public string JobSchedule { get; set; }
    public string ContentAsMarkdown { get; set; }
    public bool IsCustomerSpecific { get; set; }
}