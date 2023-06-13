using System;
using System.Collections.Generic;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class JobBuilderTest
{
    private static readonly string TestUser = "testUser";
    private static readonly Guid? TestTenant = Guid.NewGuid();

    private readonly JobBuilder _builder;
    private readonly Guid _helloJobConfigId;
    private readonly Guid _simpleJobConfigId;

    public JobBuilderTest()
    {
        var metaCatalogue = new List<IJobMetadata>();
        metaCatalogue.Add(new JobMetadata()
        {
            TemplateId = JobInfoAttribute.GetTemplateId(typeof(HelloJob)),
            JobType = typeof(HelloJob),
            ConfigType = typeof(HelloConfig),
            ParameterType = typeof(HelloParameter)
        });
        var catalogueMock = new Mock<IJobCatalogue>();
        catalogueMock.Setup(m => m.Catalogue).Returns(metaCatalogue);

        var storeMock = new Mock<IJobConfigStore>();
        var helloConfig = new HelloConfig();
        var simpleConfig = new SimplestConfig();
        _helloJobConfigId = helloConfig.Id;
        _simpleJobConfigId = simpleConfig.Id;
        storeMock.Setup(m => m.Read(It.IsAny<Guid>())).Returns<Guid>((jobConfigId) =>
        {
            if (jobConfigId == _helloJobConfigId) return helloConfig;
            if (jobConfigId == _simpleJobConfigId) return simpleConfig;
            throw new NotFoundException(jobConfigId.ToString());
        });

        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        mockServiceScope.Setup(m => m.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(m => m.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(HelloJob)))).Returns(new HelloJob());
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IUserContext)))).Returns(new ExplicitUserContext(TestUser, TestTenant));
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IServiceScopeFactory)))).Returns(mockServiceScopeFactory.Object);

        _builder = new JobBuilder(mockServiceProvider.Object, catalogueMock.Object, storeMock.Object);
    }

    [Fact]
    public void BuildTest()
    {
        IJobInstance jobInstance = _builder.BuildInstance(_helloJobConfigId, null);
        var helloJob = Assert.IsType<HelloJob>(jobInstance?.Job);

        Assert.NotNull(jobInstance);
        Assert.NotNull(helloJob);
        Assert.Equal(5, helloJob.Config.NbOfOutputs);
        Assert.Equal("Hello-Job", helloJob.Parameter.GreetingsFrom);
        Assert.Equal(TestUser, jobInstance.State.Owner);
        Assert.Equal(TestTenant, jobInstance.State.ContextTenant);
    }

    [Fact]
    public void ReBuildTest()
    {
        var jobTemplateId = new HelloJob().TemplateId;
        var mockJobState = new Mock<IJobState>();
        mockJobState.Setup(m => m.TemplateId).Returns(jobTemplateId);
        mockJobState.Setup(m => m.JobConfigId).Returns(_helloJobConfigId);
        mockJobState.Setup(m => m.Owner).Returns(TestUser);
        mockJobState.Setup(m => m.ContextTenant).Returns(TestTenant);

        IJobInstance jobInstance = _builder.ReBuildInstance(mockJobState.Object);
        var helloJob = Assert.IsType<HelloJob>(jobInstance?.Job);

        Assert.NotNull(jobInstance);
        Assert.NotNull(helloJob);
        Assert.Equal(5, helloJob.Config.NbOfOutputs);
        Assert.Equal("Hello-Job", helloJob.Parameter.GreetingsFrom);
        Assert.Equal(TestUser, jobInstance.State.Owner);
        Assert.Equal(TestTenant, jobInstance.State.ContextTenant);
    }

    [Fact]
    public void BuildUnknownTest()
    {
        Assert.Throws<NotFoundException>(() => _builder.BuildInstance(Guid.NewGuid(), null));
        Assert.Throws<NotFoundException>(() => _builder.BuildInstance(_simpleJobConfigId, null));
    }
}