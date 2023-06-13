using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.DataLayer.Settings.Context;
using IAG.ProcessEngine.DataLayer.Settings.Model;
using IAG.ProcessEngine.InternalJob.Monitoring;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

using FollowUpJob = IAG.Infrastructure.ProcessEngine.Configuration.FollowUpJob;

namespace IAG.ProcessEngine.Test.Configuration;

public class JobConfigStoreDbTest
{
    private readonly JobConfigStoreDb _store;
    private readonly SettingsDbContext _context;

    public JobConfigStoreDbTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SettingsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new SettingsDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));

        var mockCatalogue = new Mock<IJobCatalogue>();
        mockCatalogue.Setup(m => m.Catalogue).Returns(new List<IJobMetadata>
        {
            new JobMetadata
            {
                TemplateId = JobInfoAttribute.GetTemplateId(typeof(HelloJob)), 
                ConfigType = typeof(HelloConfig),
                PluginName = HelloJob.JobName
            },
            new JobMetadata
            {
                TemplateId = JobInfoAttribute.GetTemplateId(typeof(SimplestJob)), 
                ConfigType = typeof(SimplestConfig),
                PluginName = SimplestJob.JobName
            }
        });

        _store = new JobConfigStoreDb(_context, mockCatalogue.Object, null, new MockILogger<JobConfigStoreDb>());
    }

    [Fact]
    public void TestConfigJobBaseCrud()
    {
        var config = new HelloConfig()
        {
            RetryIntervals = Enumerable.Repeat(new TimeSpan(0, 2, 0), 3).ToArray(),
            Description = "InitialDescription"
        };

        // insert
        _store.Insert(config);
        Assert.Throws<DuplicateKeyException>(() => _store.Insert(config));

        // read
        IJobConfig configRead = _store.Read(config.Id);
        var helloConfigRead = Assert.IsType<HelloConfig>(configRead);
        Assert.Equal(config.Id, configRead.Id);
        Assert.Equal(config.Name, helloConfigRead.Name);
        Assert.Equal(3, helloConfigRead.RetryIntervals.Length);
        Assert.Equal(2, helloConfigRead.RetryIntervals[1].Minutes);
        Assert.Equal("InitialDescription", configRead.Description);
        Assert.Empty(configRead.FollowUpJobs);

        // update
        config.Name = "Hallo 2";
        config.Description = "UpdatedDescription";
        _store.Update(config);

        configRead = _store.Read(config.Id);
        Assert.Equal("Hallo 2", configRead.Name);
        Assert.Equal("UpdatedDescription", configRead.Description);
        Assert.Empty(configRead.FollowUpJobs);

        // delete
        _store.Delete(config);
        Assert.Throws<NotFoundException>(() => _store.Read(config.Id));
        Assert.Throws<NotFoundException>(() => _store.Update(config));
        Assert.Throws<NotFoundException>(() => _store.Delete(config));
    }

    [Fact]
    public void TestConfigFollowUpCrud()
    {
        var mainJob = new HelloConfig
        {
            RetryIntervals = Enumerable.Repeat(new TimeSpan(0, 2, 0), 3).ToArray(),
        };
        var followUpJob = new SimplestConfig();
        mainJob.FollowUpJobs.Add(new FollowUpJob { FollowUpJobConfigId = followUpJob.Id, Description = "InitialDescription", ExecutionCondition = "InitialCondition" });

        // insert
        _store.Insert(mainJob);
        Assert.Throws<DuplicateKeyException>(() => _store.Insert(mainJob));

        // read
        IJobConfig configRead = _store.Read(mainJob.Id);
        var singleFollowUp = Assert.Single(configRead.FollowUpJobs);
        Assert.NotNull(singleFollowUp);
        Assert.Equal("InitialDescription", singleFollowUp.Description);
        Assert.Equal("InitialCondition", singleFollowUp.ExecutionCondition);

        // update 1
        mainJob.Name = "Hallo 2";
        mainJob.FollowUpJobs.First().Description = "UpdatedDescription";
        mainJob.FollowUpJobs.First().ExecutionCondition = "UpdatedCondition";
        mainJob.FollowUpJobs.Add(new FollowUpJob { FollowUpJobConfigId = followUpJob.Id, Description = "SecondDescription", ExecutionCondition = "SecondCondition" });
        _store.Update(mainJob);

        configRead = _store.Read(mainJob.Id);
        Assert.Equal("Hallo 2", configRead.Name);
        Assert.Equal(2, configRead.FollowUpJobs.Count);
        Assert.Contains(configRead.FollowUpJobs, f => f.Description == "UpdatedDescription" && f.ExecutionCondition == "UpdatedCondition");
        Assert.Contains(configRead.FollowUpJobs, f => f.Description == "SecondDescription" && f.ExecutionCondition == "SecondCondition");

        // remove all followUps
        mainJob.FollowUpJobs.Clear();
        _store.Update(mainJob);

        configRead = _store.Read(mainJob.Id);
        Assert.Empty(configRead.FollowUpJobs);
    }

    [Fact]
    public void GetAllTest()
    {
        var helloConfig = new HelloConfig();
        var simplestConfig = new SimplestConfig();
        _store.Insert(helloConfig);
        _store.Insert(simplestConfig);
        _store.Insert(new MonitoringJobConfig());

        var allEntries = _store.GetAll().ToList();

        Assert.Equal(2, allEntries.Count);
    }

    [Fact]
    public void GetOrCreateJobConfigTest()
    {
        var helloConfig = new HelloConfig();
        var simplestConfig = new SimplestConfig();
        _store.Insert(helloConfig);
        _store.Insert(simplestConfig);
        _store.Insert(new SimplestConfig
        {
            Name = "Simple"
        });

        var helloByContains = _store.GetOrCreateJobConfig("hELl");
        Assert.NotNull(helloByContains);
        Assert.IsType<HelloConfig>(helloByContains);
        Assert.Equal(helloConfig.Id, helloByContains.Id);

        var simpleByContains = _store.GetOrCreateJobConfig("simple");
        Assert.NotNull(simpleByContains);
        Assert.IsType<SimplestConfig>(simpleByContains);
            
        var simpleByMeta = _store.GetOrCreateJobConfig("simpl");
        Assert.NotNull(simpleByMeta);
        Assert.IsType<SimplestConfig>(simpleByMeta);

        Assert.Throws<NotFoundException>(() => _store.GetOrCreateJobConfig("xxx"));
    }

    [Fact]
    public void ConfigWithoutMetaTest()
    {
        var config = new MonitoringJobConfig();
        _store.Insert(config);

        Assert.Throws<NotFoundException>(() => _store.Read(config.Id));
    }

    [Fact]
    public void GetFollowUpJobsTest()
    {
        var testMasterJob = new HelloConfig();
        var testFollowUpJob1 = new SimplestConfig();
        var testFollowUpJob2 = new SimplestConfig();

        _store.Insert(testMasterJob);
        _store.Insert(testFollowUpJob1);
        _store.Insert(testFollowUpJob2);

        _context.FollowUpJobs.Add(new DataLayer.Settings.Model.FollowUpJob { Id = Guid.NewGuid(), MasterId = testMasterJob.Id, FollowUpId = testFollowUpJob1.Id });
        _context.FollowUpJobs.Add(new DataLayer.Settings.Model.FollowUpJob { Id = Guid.NewGuid(), MasterId = testMasterJob.Id, FollowUpId = testFollowUpJob2.Id });
        _context.SaveChanges();

        var masterJob = _store.Read(testMasterJob.Id);
        var followUpJobs = _store.GetFollowUpJobs(testMasterJob.Id).ToList();

        Assert.NotNull(masterJob);
        Assert.NotNull(masterJob.FollowUpJobs);
        Assert.Equal(2, masterJob.FollowUpJobs.Count);
        Assert.Equal(2, followUpJobs.Count);
        Assert.Contains(followUpJobs, j => j.FollowUpJobConfigId == testFollowUpJob1.Id);
        Assert.Contains(followUpJobs, j => j.FollowUpJobConfigId == testFollowUpJob2.Id);
    }

    [Fact]
    public void ConfigJobReplacementTest()
    {
        var configIn = new HelloConfig
        {
            Description = "Test $$Text$"
        };

        _store.Insert(configIn);
        _context.ConfigCommonEntries.Add(new ConfigCommon() { Name = "Text", Data = "42" });
        _context.SaveChanges();

        IJobConfig configRead = _store.Read(configIn.Id);
        var configs = _store.GetAll();

        var configReadJob = Assert.IsType<HelloConfig>(configRead);
        Assert.Equal("Test 42", configReadJob.Description);
        configReadJob = Assert.IsType<HelloConfig>(Assert.Single(configs));
        Assert.NotNull(configReadJob);
        Assert.Equal("Test 42", configReadJob.Description);
    }

    [Fact]
    public void ConfigJobWithoutReplacementTest()
    {
        var configIn = new HelloConfig
        {
            Description = "Test $$Text$"
        };

        _store.Insert(configIn);
        _context.ConfigCommonEntries.Add(new ConfigCommon() { Name = "Text", Data = "42" });
        _context.SaveChanges();

        IJobConfig configRead = _store.ReadUnprocessed(configIn.Id);
        var configs = _store.GetAllUnprocessed();

        var configReadJob = Assert.IsType<HelloConfig>(configRead);
        Assert.Equal("Test $$Text$", configReadJob.Description);
        configReadJob = Assert.IsType<HelloConfig>(Assert.Single(configs));
        Assert.NotNull(configReadJob);
        Assert.Equal("Test $$Text$", configReadJob.Description);
    }

    [Fact]
    public void EnsureConfigTest()
    {
        var existingTemplateId = Guid.NewGuid();
        var existingConfigData = "ExistingConfig";
        _context.ConfigEntries.Add(new JobConfig()
        {
            TemplateId = existingTemplateId,
            Data = existingConfigData
        });
        _context.SaveChanges();

        var helloJobTemplateId = JobInfoAttribute.GetTemplateId(typeof(HelloJob));

        _store.EnsureConfig(existingTemplateId, typeof(HelloConfig));
        _store.EnsureConfig(helloJobTemplateId, typeof(HelloConfig));


        var existingConfig = Assert.Single(_context.ConfigEntries, c => c.TemplateId == existingTemplateId);
        var newConfig = Assert.Single(_context.ConfigEntries, c => c.TemplateId == helloJobTemplateId);
        Assert.NotNull(existingConfig);
        Assert.NotNull(newConfig);
        Assert.Equal(2, _context.ConfigEntries.Count());
        Assert.Equal(existingConfigData, existingConfig.Data);
        Assert.Contains(nameof(HelloConfig.NbOfOutputs), newConfig.Data);
    }
}