using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Execution.Condition;

public interface ICondition
{
    bool Check(IJobInstance jobInstance);
}

public class AndCondition : ICondition
{
    private readonly ICondition _firstCondition;
    private readonly ICondition _secondCondition;
        
    public AndCondition(ICondition firstCondition, ICondition secondCondition)
    {
        _firstCondition = firstCondition;
        _secondCondition = secondCondition;
    }

    public bool Check(IJobInstance jobInstance)
    {
        return _firstCondition.Check(jobInstance) && _secondCondition.Check(jobInstance);
    }
}

public class OrCondition : ICondition
{
    private readonly ICondition _firstCondition;
    private readonly ICondition _secondCondition;

    public OrCondition(ICondition firstCondition, ICondition secondCondition)
    {
        _firstCondition = firstCondition;
        _secondCondition = secondCondition;
    }

    public bool Check(IJobInstance jobInstance)
    {
        return _firstCondition.Check(jobInstance) || _secondCondition.Check(jobInstance);
    }
}


public class TrueCondition : ICondition
{
    public bool Check(IJobInstance jobInstance) => true;
}

public class FalseCondition : ICondition
{
    public bool Check(IJobInstance jobInstance) => false;
}