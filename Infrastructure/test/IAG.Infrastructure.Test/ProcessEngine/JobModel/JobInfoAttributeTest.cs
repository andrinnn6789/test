using System;

using IAG.Infrastructure.ProcessEngine.JobModel;

using Xunit;

namespace IAG.Infrastructure.Test.ProcessEngine.JobModel;

public class JobInfoAttributeTest
{
    [Fact]
    public void PluginInfoTest()
    {
        var testType = typeof(TestClass);

        Assert.NotNull(JobInfoAttribute.GetJobInfo(testType));
        Assert.Equal(new Guid("21F90665-A929-47E2-BE48-1F9DA06F470C"), JobInfoAttribute.GetTemplateId(testType));
        Assert.Equal("TestName", JobInfoAttribute.GetName(testType));
    }

    [Fact]
    public void WrongTypeWithoutAttributeTest()
    {
        var testType = typeof(string);

        Assert.Throws<System.Exception>(() => JobInfoAttribute.GetTemplateId(testType));
        Assert.Throws<System.Exception>(() => JobInfoAttribute.GetName(testType));
    }

    [JobInfo("21F90665-A929-47E2-BE48-1F9DA06F470C", "TestName")]
    private class TestClass
    { }

}