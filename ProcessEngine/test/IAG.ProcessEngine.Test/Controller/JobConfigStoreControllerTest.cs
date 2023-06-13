using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Controller;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Moq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.ProcessEngine.Test.Controller;

public class JobConfigStoreControllerTest
{
    private readonly JobConfigStoreController _controller;
    private IJobConfig _lastUpdatedConfig;
    private readonly Mock<IStringLocalizer<JobConfigStoreController>> _localizer;
    private readonly Mock<IConditionChecker> _conditionChecker;
    private readonly Guid _helloJobConfigId;
    private readonly List<IJobConfig> _configStore;

    public JobConfigStoreControllerTest()
    {
        var testConfig = new HelloConfig();
        var mockCatalogue = new Mock<IJobCatalogue>();
        mockCatalogue.Setup(m => m.Catalogue).Returns(new List<IJobMetadata> {new JobMetadata
        {
            TemplateId = testConfig.TemplateId, ConfigType = typeof(HelloConfig)
        }});

        var helloConfig = new HelloConfig();
        var simpleConfig = new SimplestConfig();
        _configStore = new List<IJobConfig> {helloConfig, simpleConfig};
        _helloJobConfigId = helloConfig.Id;
        var mockConfigStore = new Mock<IJobConfigStore>();
        mockConfigStore.Setup(m => m.Read(It.IsAny<Guid>())).Returns<Guid>(jobConfigId =>
        {
            var config = _configStore.FirstOrDefault(c => c.Id == jobConfigId);
            if (config == null)
            {
                throw new NotFoundException(jobConfigId.ToString());
            }

            return config;
        });
        mockConfigStore.Setup(m => m.ReadUnprocessed(It.IsAny<Guid>())).Returns<Guid>(jobConfigId =>
        {
            var config = _configStore.FirstOrDefault(c => c.Id == jobConfigId);
            if (config == null)
            {
                throw new NotFoundException(jobConfigId.ToString());
            }

            return config;
        });
        mockConfigStore.Setup(m => m.Update(It.IsAny<IJobConfig>())).Callback<IJobConfig>(config =>
        {
            if (_configStore.All(c => c.Id != config.Id))
            {
                throw new NotFoundException(config.Id.ToString());
            }

            _lastUpdatedConfig = config;
        });
        mockConfigStore.Setup(m => m.Insert(It.IsAny<IJobConfig>())).Callback<IJobConfig>(config =>
        {
            if (_configStore.Any(c => c.Id == config.Id))
            {
                throw new DuplicateKeyException(config.Id.ToString());
            }

            _configStore.Add(config);
        });
        mockConfigStore.Setup(m => m.Delete(It.IsAny<IJobConfig>())).Callback<IJobConfig>(config =>
        {
            _configStore.RemoveAll(c => c.Id == config.Id);
        });
        mockConfigStore.Setup(m => m.GetAll()).Returns(_configStore);
        mockConfigStore.Setup(m => m.GetAllUnprocessed()).Returns(_configStore);

        _controller = new JobConfigStoreController(mockConfigStore.Object, mockCatalogue.Object)
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };

        _localizer = new Mock<IStringLocalizer<JobConfigStoreController>>();
        _conditionChecker = new Mock<IConditionChecker>();
    }

    [Fact]
    public void GetAllTest()
    {
        var result = _controller.GetAll();
        var configs = Assert.IsType<List<IJobConfig>>(result?.Value);

        Assert.NotNull(configs);
        Assert.Contains(configs, c => c.GetType() == typeof(HelloConfig));
        Assert.Contains(configs, c => c.GetType() == typeof(SimplestConfig));
    }

    [Fact]
    public void GetAllUnprocessedTest()
    {
        var result = _controller.GetAllUnprocessed();
        var configs = Assert.IsType<List<IJobConfig>>(result?.Value);

        Assert.NotNull(configs);
        Assert.Contains(configs, c => c.GetType() == typeof(HelloConfig));
        Assert.Contains(configs, c => c.GetType() == typeof(SimplestConfig));
    }

    [Fact]
    public void GetExistingTest()
    {
        var result = _controller.GetById(_helloJobConfigId);

        Assert.NotNull(result);
        Assert.IsType<HelloConfig>(result.Value);
    }

    [Fact]
    public void GetNotExistingTest()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void GetExistingUnprocessedTest()
    {
        var result = _controller.GetByIdUnprocessed(_helloJobConfigId);

        Assert.NotNull(result);
        Assert.IsType<HelloConfig>(result.Value);
    }

    [Fact]
    public void GetNotExistingUnprocessedTest()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetByIdUnprocessed(Guid.NewGuid()));
    }

    [Fact]
    public void GetForTemplateIdTest()
    {
        var result = _controller.GetForTemplate(JobInfoAttribute.GetTemplateId(typeof(HelloJob)));

        Assert.NotNull(result);
        Assert.IsType<HelloConfig>(result.Value);
    }

    [Fact]
    public void GetForUnknownTemplateIdTest()
    {
        var result = _controller.GetForTemplate(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void CreateWithDeleteTest()
    {
        var config = new HelloConfig { NbOfOutputs = 42 };
        var result = _controller.Create(JObject.FromObject(config), _localizer.Object, _conditionChecker.Object);
        var createdConfig = Assert.IsType<CreatedResult>(result).Value as HelloConfig;

        Assert.NotNull(createdConfig);
        Assert.Equal(config.Id, createdConfig.Id);
        Assert.Equal(config.TemplateId, createdConfig.TemplateId);
        Assert.Equal(config.NbOfOutputs, createdConfig.NbOfOutputs);
            
        _controller.Delete(createdConfig.Id);
        Assert.Throws<NotFoundException>(() => _controller.GetById(createdConfig.Id));
    }

    [Fact]
    public void CreateWithInvalidCronExpressionTest()
    {
        var config = JObject.FromObject(new HelloConfig { NbOfOutputs = 42 });
        config["CronExpression"] = "xx";
        Assert.Throws<JsonSerializationException>(() => _controller.Create(config, _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void CreateWithInvalidFollowUpConditionTest()
    {
        _conditionChecker.Setup(m => m.CheckConditionValidity(It.IsAny<string>())).Callback<string>(c =>
        {
            if (c == "Invalid")
            {
                throw new ParseException("TestException");
            }
        });

        var config = new HelloConfig { NbOfOutputs = 42 };
        config.FollowUpJobs.Add(new FollowUpJob { ExecutionCondition = "Valid" });
        config.FollowUpJobs.Add(new FollowUpJob { ExecutionCondition = "Invalid" });
        Assert.Throws<BadRequestException>(() => _controller.Create(JObject.FromObject(config), _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void CreateWithUnknownConfigTest()
    {
        var config = new SimplestConfig();
        Assert.Throws<NotFoundException>(() => _controller.Create(JObject.FromObject(config), _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void CreateWithInvalidConfigTest()
    {
        Assert.Throws<BadRequestException>(() => _controller.Create(null, null, null));
        Assert.Throws<BadRequestException>(() => _controller.Create(JObject.FromObject(this), null, null));
    }

    [Fact]
    public void UpdateTest()
    {
        var config = new HelloConfig { NbOfOutputs = 42 };
        var result = _controller.Update(_helloJobConfigId, JObject.FromObject(config), _localizer.Object, _conditionChecker.Object);

        Assert.IsType<NoContentResult>(result);
        var updatedConfig = Assert.IsType<HelloConfig>(_lastUpdatedConfig);
        Assert.Equal(config.NbOfOutputs, updatedConfig.NbOfOutputs);
    }

    [Fact]
    public void UpdateWithInvalidCronExpressionTest()
    {
        var config = JObject.FromObject(new HelloConfig { NbOfOutputs = 42 });
        config["CronExpression"] = "xx";
        Assert.Throws<JsonSerializationException>(() => _controller.Update(_helloJobConfigId, config, _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void UpdateWithInvalidFollowUpConditionTest()
    {
        _conditionChecker.Setup(m => m.CheckConditionValidity(It.IsAny<string>())).Callback<string>(c =>
        {
            if (c == "Invalid")
            {
                throw new ParseException("TestException");
            }
        });

        var config = new HelloConfig { NbOfOutputs = 42 };
        config.FollowUpJobs.Add(new FollowUpJob { ExecutionCondition = "Valid" });
        config.FollowUpJobs.Add(new FollowUpJob { ExecutionCondition = "Invalid" });
        Assert.Throws<BadRequestException>(() => _controller.Update(_helloJobConfigId, JObject.FromObject(config), _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void UpdateNotExistingTest()
    {
        var config = new HelloConfig();
        Assert.Throws<NotFoundException>(() => _controller.Update(Guid.NewGuid(), JObject.FromObject(config), _localizer.Object, _conditionChecker.Object));
    }

    [Fact]
    public void UpdateWithInvalidConfigTest()
    {
        Assert.Throws<BadRequestException>(() => _controller.Update(_helloJobConfigId, null, null, null));
        Assert.Throws<BadRequestException>(() => _controller.Update(_helloJobConfigId, JObject.FromObject(this), null, null));
    }

    [Fact]
    public void InitConfigsFromEmptyTest()
    {
        _configStore.Clear();

        var result = _controller.InitConfigs();
        var newConfig = new HelloConfig();

        Assert.IsType<OkResult>(result);
        var singleConfig = Assert.Single(_configStore);
        Assert.NotNull(singleConfig);
        Assert.Equal(newConfig.TemplateId, singleConfig.TemplateId);
        Assert.Equal(newConfig.TemplateId, singleConfig.Id);
        var initConfig = Assert.IsType<HelloConfig>(singleConfig);
        Assert.Equal(newConfig.Delay, initConfig.Delay);
        Assert.Equal(newConfig.NbOfOutputs, initConfig.NbOfOutputs);
    }

    [Fact]
    public void InitConfigsWithExistingTest()
    {
        var initialCount = _configStore.Count;
        var result = _controller.InitConfigs();

        Assert.IsType<OkResult>(result);
        Assert.Equal(initialCount, _configStore.Count);
        Assert.All(_configStore, c => Assert.NotEqual(c.Id, c.TemplateId));
    }

    [Fact]
    public void CleanUpConfigsTest()
    {
        var result = _controller.CleanUpConfigs();
        var newConfig = new HelloConfig();

        Assert.IsType<OkResult>(result);
        var singleConfig = Assert.Single(_configStore);
        Assert.NotNull(singleConfig);
        Assert.Equal(newConfig.TemplateId, singleConfig.TemplateId);
        Assert.NotEqual(newConfig.TemplateId, singleConfig.Id);
        Assert.IsType<HelloConfig>(singleConfig);
    }
}