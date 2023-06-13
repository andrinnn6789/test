using System;
using System.Collections.Generic;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.AspNetCore.Mvc;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Models;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Controllers;

public class JobDocsControllerTest
{
    private readonly JobDocsController _controller;

    private static readonly Guid Job1TemplateId = Guid.Parse("BB43C563-D737-4151-B14B-2BEC09C71115");
    private const string Job1Name = "VinX.OrderBravoJob";
    private static readonly Guid Job2TemplateId = Guid.Parse("725E0C8F-C79B-4300-A270-C43EFCDFBC1B");
    private const string Job2Name = "VinX.DigitalDrinkJob";

    private readonly List<JobDoc> _jobDocs = new()
    {
        new JobDoc
        {
            TemplateId = Job1TemplateId,
            JobName = Job1Name,
            IsJobActive = true,
            JobSchedule = "Jede Minute",
            ContentAsMarkdown = "",
            IsCustomerSpecific = false
        },
        new JobDoc
        {
            TemplateId = Job2TemplateId,
            JobName = Job2Name,
            IsJobActive = false,
            JobSchedule = "Um 12:00",
            ContentAsMarkdown = "",
            IsCustomerSpecific = false
        }
    };

    public JobDocsControllerTest()
    {
        var pluginLoader = SetupFakePluginLoader();
        _controller = new JobDocsController(pluginLoader)
        {
            ControllerContext = new ControllerContext(new ActionContext(new DefaultHttpContext(), new RouteData(),
                new ControllerActionDescriptor()))
        };
    }
    
    [Fact]
    public void Index_WhenExecuted_ShouldReturnDocsViewModel()
    {
        // Arrange
        var result = _controller.Index();

        // Act & Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsType<JobDocsViewModel>(viewResult.Model);

        var jobDoc1 = viewModel.Docs[0];
        var jobDoc2 = viewModel.Docs[1];
        
        using (new AssertionScope())
        {
            Assert.Null(viewModel.ErrorMessage);
            Assert.Equal(2, viewModel.Docs.Count);
            jobDoc1.Should().BeEquivalentTo(_jobDocs[0]);
            jobDoc2.Should().BeEquivalentTo(_jobDocs[1]);
        }
    }
    
    [Fact]
    public void Error_WhenExecuted_ShouldReturnErrorViewModel()
    {
        // Arrange
        var result = _controller.Error();

        // Act & Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model.RequestId);
        Assert.NotEmpty(model.RequestId);
    }

    private static IPluginLoader SetupFakePluginLoader()
    {
        var fakePluginLoader = new Mock<IPluginLoader>();
        fakePluginLoader.Setup(p => p.GetImplementations<IJobConfigProvider>(null, false))
            .Returns(new[] { typeof(FakeJobConfigProvider) });
        fakePluginLoader.Setup(p => p.GetTypesWithAttribute(typeof(JobInfoAttribute), null)).Returns(new[] { typeof(FakeOrderBravoJob), typeof(FakeDigitalDrinkJob) });
        

        return fakePluginLoader.Object;
    }
}

public class FakeJobConfig : IJobConfig
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SerializedData { get; set; }
    public bool Active { get; set; }
    public bool AllowConcurrentInstances { get; set; }
    public TimeSpan HeartbeatTimeout { get; set; }
    public bool LogActivity { get; set; }
    public LogLevel LogLevel { get; set; }
    public TimeSpan[] RetryIntervals { get; set; }
    public string CronExpression { get; set; }
    public List<FollowUpJob> FollowUpJobs { get; set; }
}

[JobInfo("BB43C563-D737-4151-B14B-2BEC09C71115", "VinX.OrderBravoJob")]
public class FakeOrderBravoJob
{
}

[JobInfo("725E0C8F-C79B-4300-A270-C43EFCDFBC1B", "VinX.DigitalDrinkJob")]
public class FakeDigitalDrinkJob
{
}

public class FakeJobConfigProvider : IJobConfigProvider
{
    public IEnumerable<IJobConfig> GetAll()
    {
        return new List<IJobConfig>()
        {
            new FakeJobConfig
            {
                TemplateId = Guid.Parse("BB43C563-D737-4151-B14B-2BEC09C71115"),
                Active = true,
                CronExpression = "* * * * *"
            },
            new FakeJobConfig
            {
                TemplateId = Guid.Parse("725E0C8F-C79B-4300-A270-C43EFCDFBC1B"),
                Active = false,
                CronExpression = "0 12 * * *"
            }
        };
    }
}