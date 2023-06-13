using System.Collections.Generic;

using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Execution.Condition;

public class ConditionChecker : IConditionChecker
{
    private readonly IConditionParser _parser;
    private readonly Dictionary<string, ICondition> _parsedConditions;

    public ConditionChecker(IConditionParser parser)
    {
        _parser = parser;
        _parsedConditions = new Dictionary<string, ICondition>();
    }

    public void CheckConditionValidity(string conditionString)
    {
        GetCondition(conditionString);
    }

    public bool CheckCondition(IJobInstance jobInstance, string conditionString)
    {
        var condition = GetCondition(conditionString);

        return condition.Check(jobInstance);
    }

    private ICondition GetCondition(string conditionString)
    {
        if (string.IsNullOrWhiteSpace(conditionString))
            return new TrueCondition();

        if (!_parsedConditions.ContainsKey(conditionString))
        {
            _parsedConditions.Add(conditionString, _parser.Parse(conditionString));
        }

        return _parsedConditions[conditionString];
    }
}