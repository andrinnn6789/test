namespace IAG.ProcessEngine.Execution.Condition;

public interface IConditionParser
{
    ICondition Parse(string condition);
}