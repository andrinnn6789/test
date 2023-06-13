using System;
using System.Collections.Generic;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Configuration;

public class JobCatalogueTest
{
    private readonly JobCatalogue _jobCatalogue;

    public JobCatalogueTest()
    {
        var pluginLoader = new Mock<IPluginLoader>();
        var types = new List<Type> { typeof(HelloJob), typeof(SimplestJob), typeof(object) };

        pluginLoader.Setup(m => m.GetImplementations<IJob>(It.IsAny<string>(), It.IsAny<bool>())).Returns(types);
        _jobCatalogue = new JobCatalogue(pluginLoader.Object, new MockILogger<JobCatalogue>());
    }

    [Fact]
    public void JobCatalogueListTest()
    {
        Assert.NotEmpty(_jobCatalogue.Catalogue);
        Assert.Equal(2, _jobCatalogue.Catalogue.Count);
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.True(jobMeta.TemplateId != Guid.Empty));
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.NotNull(jobMeta.PluginName));
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.NotEmpty(jobMeta.PluginName));
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.NotNull(jobMeta.JobType));
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.NotNull(jobMeta.ConfigType));
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.NotNull(jobMeta.ParameterType));
    }

    [Fact]
    public void JobCatalogueReloadTest()
    {
        var catalogueBeforeReload = new List<IJobMetadata>(_jobCatalogue.Catalogue);
        _jobCatalogue.Reload();
            
        Assert.Equal(catalogueBeforeReload.Count, _jobCatalogue.Catalogue.Count);
        Assert.All(_jobCatalogue.Catalogue, jobMeta => Assert.Single(catalogueBeforeReload, m => m.TemplateId == jobMeta.TemplateId));
    }
}