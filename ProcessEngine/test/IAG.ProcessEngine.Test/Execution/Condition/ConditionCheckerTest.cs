using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Execution.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution.Condition;

public class ConditionCheckerTest
{
    private readonly IConditionChecker _checker;
    private bool _conditionEvaluatesTo;
    private bool _throwParseException;

    public ConditionCheckerTest()
    {
        var condition = new Mock<ICondition>();
        condition.Setup(m => m.Check(It.IsAny<IJobInstance>())).Returns(() => _conditionEvaluatesTo);

        var parser = new Mock<IConditionParser>();
        parser.Setup(m => m.Parse(It.IsAny<string>())).Returns(() =>
        {
            if (_throwParseException) throw new ParseException(string.Empty);
            return condition.Object;
        });

        _checker = new ConditionChecker(parser.Object);
    }

    [Fact]
    public void CheckConditionValidityTest()
    {
        _throwParseException = false;
        _checker.CheckConditionValidity(null);
        _checker.CheckConditionValidity(string.Empty);
        _checker.CheckConditionValidity("    ");
        _checker.CheckConditionValidity("DoesNotMatter");
        _throwParseException = true;
        _checker.CheckConditionValidity("DoesNotMatter"); // should not throw since it is taken from cache

        Assert.Throws<ParseException>(() => _checker.CheckConditionValidity("DoesMatterALittleBit"));
    }

    [Fact]
    public void CheckConditionTest()
    {
        _throwParseException = false;
        _conditionEvaluatesTo = false;
        var falseResult = _checker.CheckCondition(null, "DoesNotMatter");
        _conditionEvaluatesTo = true;
        var trueResult = _checker.CheckCondition(null, "DoesStillNotMatter");
        var nullResult = _checker.CheckCondition(null, null);
        var emptyResult = _checker.CheckCondition(null, string.Empty);
        var spacesResult =_checker.CheckCondition(null, "    ");
        _throwParseException = true;
        _checker.CheckCondition(null, "DoesNotMatter"); // should not throw since it is taken from cache

        Assert.False(falseResult);
        Assert.True(trueResult);
        Assert.True(nullResult);
        Assert.True(emptyResult);
        Assert.True(spacesResult);
        Assert.Throws<ParseException>(() => _checker.CheckCondition(null, "DoesMatterALittleBit"));
    }
}