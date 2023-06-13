using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Execution.Condition;

public interface IConditionChecker
{
    void CheckConditionValidity(string condition);

    bool CheckCondition(IJobInstance jobInstance, string condition);
}