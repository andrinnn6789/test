using System;

using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Execution.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution.Condition;

public class OperationPartTest
{
    [Fact]
    public void ConstantPartTest()
    {
        var part42 = new OperationPart("42");
        var partTestString = new OperationPart("\"Test\"");
        var partNumbers = new OperationPart("42|23|17");
        var partStrings = new OperationPart("\"Hello\"|\"World\"");

        var numbers = partNumbers.GetValue(null);
        var strings = partStrings.GetValue(null);

        Assert.Equal("42", Assert.Single(part42.GetValue(null)));
        Assert.Equal("Test", Assert.Single(partTestString.GetValue(null)));
        Assert.NotNull(numbers);
        Assert.NotNull(strings);
        Assert.Equal(3, numbers.Count);
        Assert.Equal(2, strings.Count);
        Assert.Contains("42", numbers);
        Assert.Contains("23", numbers);
        Assert.Contains("17", numbers);
        Assert.Contains("Hello", strings);
        Assert.Contains("World", strings);
    }

    [Fact]
    public void StatusPartTest()
    {
        var partJobResult = new OperationPart("JobResult");
        var partExecutionState = new OperationPart("ExecutionState");
        var mockJobInstance = new Mock<IJobInstance>();
        var mockJobState = new Mock<IJobState>();
        var mockJobResult = new Mock<IJobResult>();
        mockJobInstance.Setup(m => m.State).Returns(mockJobState.Object);
        mockJobState.Setup(m => m.Result).Returns(mockJobResult.Object);
        mockJobState.Setup(m => m.ExecutionState).Returns(JobExecutionStateEnum.Success);
        mockJobResult.Setup(m => m.Result).Returns(JobResultEnum.PartialSuccess);

        Assert.Equal(((int)JobResultEnum.PartialSuccess).ToString(), Assert.Single(partJobResult.GetValue(mockJobInstance.Object)));
        Assert.Equal(((int)JobExecutionStateEnum.Success).ToString(), Assert.Single(partExecutionState.GetValue(mockJobInstance.Object)));
    }

    [Fact]
    public void InvalidPartTest()
    {
        Assert.Throws<ArgumentNullException>(() => new OperationPart(null));
        Assert.Throws<ParseException>(() => new OperationPart(string.Empty));
        Assert.Throws<ParseException>(() => new OperationPart("InvalidConstant"));
        Assert.Throws<ParseException>(() => new OperationPart("Constant"));
        Assert.Throws<ParseException>(() => new OperationPart("42|InvalidValue"));
    }
}