using System;

namespace IAG.Infrastructure.ProcessEngine.Configuration;

public interface IFollowUpJob
{
    Guid Id { get; set; }
    Guid FollowUpJobConfigId { get; set; }
    string ExecutionCondition { get; set; }
    string Description { get; set; }
}