using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Execution.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution.Condition;

public class CompareConditionTest
{
    [Fact]
    public void ConstantCompareTest()
    {
        Assert.True(new CompareCondition("42", CompareOperator.Equal, "42").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.Equal, "0|42|23|17").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.NotEqual, "23").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.NotEqual, "0|23|17").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.Lower, "77").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.Lower, "420").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.LowerEqual, "77").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.LowerEqual, "420").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.LowerEqual, "42").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.Greater, "23").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.Greater, "4").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.GreaterEqual, "23").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.GreaterEqual, "4").Check(null));
        Assert.True(new CompareCondition("42", CompareOperator.GreaterEqual, "42").Check(null));

        Assert.False(new CompareCondition("42", CompareOperator.Equal, "23").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Equal, "0|23|17").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.NotEqual, "42").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.NotEqual, "0|42|23|17").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Lower, "23").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Lower, "4").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Lower, "42").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.LowerEqual, "23").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.LowerEqual, "4").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.LowerEqual, "41").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Greater, "77").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Greater, "420").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.Greater, "42").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.GreaterEqual, "77").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.GreaterEqual, "420").Check(null));
        Assert.False(new CompareCondition("42", CompareOperator.GreaterEqual, "43").Check(null));
    }

    [Fact]
    public void StatusCompareTest()
    {
        var mockJobInstance = new Mock<IJobInstance>();
        var mockJobState = new Mock<IJobState>();
        var mockJobResult = new Mock<IJobResult>();
        mockJobInstance.Setup(m => m.State).Returns(mockJobState.Object);
        mockJobState.Setup(m => m.Result).Returns(mockJobResult.Object);
        mockJobState.Setup(m => m.ExecutionState).Returns(JobExecutionStateEnum.Success);
        mockJobResult.Setup(m => m.Result).Returns(JobResultEnum.PartialSuccess);

        Assert.True(new CompareCondition("JobResult", CompareOperator.Equal, ((int)JobResultEnum.PartialSuccess).ToString()).Check(mockJobInstance.Object));
        Assert.True(new CompareCondition("ExecutionState", CompareOperator.Equal, ((int)JobExecutionStateEnum.Success).ToString()).Check(mockJobInstance.Object));
        Assert.False(new CompareCondition("JobResult", CompareOperator.Equal, ((int)JobResultEnum.Failed).ToString()).Check(mockJobInstance.Object));
        Assert.False(new CompareCondition("ExecutionState", CompareOperator.Equal, ((int)JobExecutionStateEnum.Running).ToString()).Check(mockJobInstance.Object));
    }

    [Fact]
    public void InvalidOperatorTest()
    {
        Assert.Throws<ParseException>(() => new CompareCondition("42", (CompareOperator)42, "42").Check(null));
    }
}