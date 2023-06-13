using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.ProcessEngine.Configuration;

[ExcludeFromCodeCoverage]
public class FollowUpJob : IFollowUpJob
{
    [UsedImplicitly]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public Guid FollowUpJobConfigId { get; set; }

    [UsedImplicitly]
    public string ExecutionCondition { get; set; }

    [UsedImplicitly]
    public string Description { get; set; }

    public FollowUpJob()
    {
        Id = Guid.NewGuid();
    }
}