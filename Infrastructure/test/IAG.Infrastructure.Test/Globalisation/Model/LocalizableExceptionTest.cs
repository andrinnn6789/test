using System;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Localizer;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Model;

public class LocalizableExceptionTest
{
    [Fact]
    public void ConstructorTest()
    {
        var testResourceId = "Test.Resource.Id";
        var testException = new LocalizableException(testResourceId, "foo", 42);

        Assert.NotNull(testException);
        Assert.NotNull(testException.LocalizableParameter);
        Assert.NotNull(testException.LocalizableParameter.ResourceId);
        Assert.NotNull(testException.LocalizableParameter.Params);
        Assert.Equal(2, testException.LocalizableParameter.Params.Length);
        Assert.Equal(testResourceId, testException.LocalizableParameter.ResourceId);
        Assert.Equal("foo", testException.LocalizableParameter.Params[0]);
        Assert.Equal(42, testException.LocalizableParameter.Params[1]);
    }

    [Fact]
    public void GetExceptionMessageTest()
    {
        var testResourceId = "Test.Resource.Id";
        var testException = new LocalizableException(testResourceId, "foo", 42);

        var exceptionMsg = LocalizableException.GetExceptionMessage(testException);

        Assert.NotNull(testException.LocalizableParameter);
        Assert.Equal(testException.LocalizableParameter, exceptionMsg);
    }

    [Fact]
    public void GetAggregateExceptionMessageTest()
    {
        var testMessage = "Hello World";
        var testAggregateException = new AggregateException(new System.Exception(testMessage));

        var exceptionMsg = LocalizableException.GetExceptionMessage(testAggregateException);

        Assert.NotNull(testAggregateException);
        Assert.NotNull(exceptionMsg);
        Assert.Contains(testMessage, exceptionMsg.ResourceId);
    }

    [Fact]
    public void GetAggregateWithLocalizableExceptionMessageTest()
    {
        var testResourceId = "Test.Resource.Id {0} {1}";
        var testResourceId2 = "Test.Resource.Id 2 {0}";
        var testMessage = "Hello World";
        var localizableException = new LocalizableException(testResourceId, "foo", 42);
        var localizableException2 = new LocalizableException(testResourceId2, 2);
        var testAggregateException = new AggregateException(
            localizableException, new System.Exception(testMessage), localizableException2);

        var exceptionMsg = LocalizableException.GetExceptionMessage(testAggregateException);

        Assert.NotNull(testAggregateException);
        Assert.NotNull(exceptionMsg);
        Assert.Contains("{0}", exceptionMsg.ResourceId);
        Assert.Equal(3, exceptionMsg.Params.Length);
        var resourceId = (exceptionMsg.Params[1] as LocalizableParameter)?.ResourceId;
        Assert.Equal(resourceId, testMessage);
        Assert.Equal(localizableException.LocalizableParameter, exceptionMsg.Params[0]);
        Assert.Equal(localizableException2.LocalizableParameter, exceptionMsg.Params[2]);
    }

    [Fact]
    public void GetInnerExceptionMessageTest()
    {
        var testMessage = "Hello World";
        var innerTestMessage = "Inside is Hell";
        var testException = new System.Exception(testMessage, new System.Exception(innerTestMessage));

        var exceptionMsg = LocalizableException.GetExceptionMessage(testException);

        Assert.NotNull(testException);
        Assert.NotNull(exceptionMsg);
        Assert.Equal(2, exceptionMsg.Params.Length);
        Assert.Equal(testMessage, (exceptionMsg.Params[1] as LocalizableParameter)?.ResourceId);
        Assert.Equal(innerTestMessage, (exceptionMsg.Params[0] as LocalizableParameter)?.ResourceId);
    }
}