using IAG.ProcessEngine.Execution.Condition;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution.Condition;

public class BaseConditionTest
{
    [Fact]
    public void AndConditionTest()
    {
        Assert.True(new AndCondition(new TrueCondition(), new TrueCondition()).Check(null));
        Assert.False(new AndCondition(new FalseCondition(), new TrueCondition()).Check(null));
        Assert.False(new AndCondition(new TrueCondition(), new FalseCondition()).Check(null));
        Assert.False(new AndCondition(new FalseCondition(), new FalseCondition()).Check(null));
    }

    [Fact]
    public void OrConditionTest()
    {
        Assert.True(new OrCondition(new TrueCondition(), new TrueCondition()).Check(null));
        Assert.True(new OrCondition(new FalseCondition(), new TrueCondition()).Check(null));
        Assert.True(new OrCondition(new TrueCondition(), new FalseCondition()).Check(null));
        Assert.False(new OrCondition(new FalseCondition(), new FalseCondition()).Check(null));
    }

    [Fact]
    public void TrueConditionTest()
    {
        Assert.True(new TrueCondition().Check(null));
    }

    [Fact]
    public void FalseConditionTest()
    {
        Assert.False(new FalseCondition().Check(null));
    }
}