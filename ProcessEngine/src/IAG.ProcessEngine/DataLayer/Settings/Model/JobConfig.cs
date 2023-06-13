using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Configuration.Model;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.DataLayer.Settings.Model;

public class JobConfig : ConfigBase
{
    [UsedImplicitly]
    public Guid TemplateId { get; set; }

    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public string Description { get; set; }

    [UsedImplicitly]
    [ForeignKey("MasterId")]
    public List<FollowUpJob> FollowUpJobs { get; set; }
}