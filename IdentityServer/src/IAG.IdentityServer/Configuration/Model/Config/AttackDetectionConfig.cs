using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Configuration.Model.Config;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class AttackDetectionConfig : IAttackDetectionConfig
{
    public static readonly string ConfigName = "IdentityServer.AttackDetection";

    public AttackDetectionConfig()
    {
        ObservationPeriod = TimeSpan.FromMinutes(1);
        MaxFailedRequests = 3;
    }

    public TimeSpan ObservationPeriod { get; set; }
    public int MaxFailedRequests { get; set; }
}