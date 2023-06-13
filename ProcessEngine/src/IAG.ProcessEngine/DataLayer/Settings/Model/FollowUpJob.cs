using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.DataLayer.Settings.Model;

[ExcludeFromCodeCoverage]
public class FollowUpJob : BaseEntity
{
    [UsedImplicitly]
    public Guid MasterId { get; set; }

    [UsedImplicitly]
    public Guid FollowUpId { get; set; }

    [UsedImplicitly]
    public string ExecutionCondition { get; set; }

    [UsedImplicitly]
    public string Description { get; set; }
}