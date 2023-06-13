using System;
using System.Reflection;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation;

public class MessageStructureTest
{
    [Fact]
    public void ConstructorTest()
    {
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "Test", 1);
        Assert.Equal(MessageTypeEnum.Information, msgStruct.Type);
        Assert.Equal("Test", msgStruct.ResourceId);
        Assert.Single(msgStruct.Params);
    }

    [Fact]
    public void JsonConstructorTest()
    {
        var timestamp = DateTime.UtcNow.AddSeconds(-7);
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "Test", timestamp);
        Assert.Equal(MessageTypeEnum.Information, msgStruct.Type);
        Assert.Equal("Test", msgStruct.ResourceId);
        Assert.Equal(timestamp, msgStruct.Timestamp);
        Assert.Null(msgStruct.Params);
    }

    [Fact]
    public void ExceptionConstructorTest()
    {
        var msgStruct = new MessageStructure(new System.Exception("Test"));
        Assert.Equal(MessageTypeEnum.Debug, msgStruct.Type);
        Assert.StartsWith("Test", msgStruct.ResourceId);
    }

    [Fact]
    public void LocalizationExceptionConstructorTest()
    {
        var msg0 = "InnerTestWithParam {0} {1}";
        var msg1 = "Test";
        var testInnerEx = new LocalizableException(msg0, 42, 3);
        var testEx = new ExceptionWithStack(msg1, testInnerEx);
        var msgStruct = new MessageStructure(testEx);
        var messages = msgStruct.ResourceId.Split(Environment.NewLine);

        Assert.Equal(MessageTypeEnum.Debug, msgStruct.Type);
        Assert.Equal(3, messages.Length);  
        Assert.Equal("{0}", messages[0]);
        Assert.Equal("{1}", messages[1]);

        Assert.True(msgStruct.Params.Length == 3);
        var locParam0 = Assert.IsAssignableFrom<ILocalizableObject>(msgStruct.Params[0]);
        var locParam1 = Assert.IsAssignableFrom<ILocalizableObject>(msgStruct.Params[1]);

        Assert.Equal(msg1, locParam1.ResourceId);
        Assert.Empty(locParam1.Params);
        Assert.Equal(msg0, locParam0.ResourceId);
        Assert.NotNull(locParam0.Params);
        Assert.Equal(2, locParam0.Params.Length);
        Assert.Equal(42, locParam0.Params[0]);
        Assert.Equal(3, locParam0.Params[1]);
        Assert.Equal(testEx.StackTrace, msgStruct.Params[2]);
    }

    private class ExceptionWithStack : LocalizableException
    {
        public ExceptionWithStack(string resourceId, System.Exception innerException, params object[] args) : base(resourceId, innerException, args)
        {
            SetStackTrace();
        }

        private void SetStackTrace()
        {
            var stackTraceField = typeof(System.Exception)
                .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField!.SetValue(this, Environment.StackTrace);
        }
    }
}