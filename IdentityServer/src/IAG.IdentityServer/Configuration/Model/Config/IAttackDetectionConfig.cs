using System;

namespace IAG.IdentityServer.Configuration.Model.Config;

public interface IAttackDetectionConfig
{
    TimeSpan ObservationPeriod { get; }
    int MaxFailedRequests { get; }
}