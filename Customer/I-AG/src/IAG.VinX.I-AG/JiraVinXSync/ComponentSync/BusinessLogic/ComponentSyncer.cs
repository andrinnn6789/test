using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using System;
using System.Collections.Generic;
using System.Linq;

using IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.Jira;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.VinX;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.Jira;
using IAG.VinX.IAG.Resource;

using Component = IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.VinX.Component;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.BusinessLogic;

public class ComponentSyncer : IComponentSyncer
{
    private readonly IComponentSyncVinXConnector _vinXConnector;

    private ComponentSyncJiraConnector _jiraConnector;
    private IMessageLogger _messageLogger;

    public ComponentSyncer(IComponentSyncVinXConnector vinXConnector)
    {
        _vinXConnector = vinXConnector;
    }

    public void SetConfig(
        string sybaseConnectionString,
        HttpConfig httpConfig,
        IMessageLogger messageLogger)
    {
        _vinXConnector.CreateConnection(sybaseConnectionString);
        var requestLogger = new RequestResponseLogger(_messageLogger);
        _jiraConnector = new ComponentSyncJiraConnector(httpConfig, requestLogger);
        _messageLogger = messageLogger;
    }

    public IJobResult SyncComponents()
    {
        var result = new JobResult
        {
            Result = JobResultEnum.Success
        };

        try
        {
            var components = _vinXConnector
                .GetComponents()
                .Where(c => !c.IsSyncedInJira)
                .ToList();

            var projects = _jiraConnector
                .GetProjects().Result
                .Where(o => o.ProjectCategory.Name != "Interne Projekte")
                .ToList();

            foreach (var component in components)
            {
                CreateComponentInJiraProjects(component, projects);
                ActivateFlagIsSyncedInVinX(component);
            }
        }
        catch (Exception ex)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.LoadComponentsToJiraErrorFormatMessage,
                LocalizableException.GetExceptionMessage(ex));
            _messageLogger.AddMessage(ex);

            result.ErrorCount++;
            result.Result = JobResultEnum.Failed;
        }
        finally
        {
            _vinXConnector.Dispose();
            _jiraConnector.Dispose();
        }

        return result;
    }

    private void CreateComponentInJiraProjects(Component component, List<Project> projects)
    {
        foreach (var project in projects)
        {
            var createComponentRequestModel = new CreateComponentRequestModel
            {
                Name = component.Name,
                AssigneeType = "UNASSIGNED",
                Project = project.Key
            };

            var projectId = Convert.ToInt32(project.Id);
            var jiraComponents = _jiraConnector.GetComponentsFromProject(projectId).Result.ToList();

            if (jiraComponents.Any(jiraComponent => jiraComponent.Name == component.Name))
                continue;

            _ = _jiraConnector.CreateComponent(createComponentRequestModel).Result;
        }
    }

    private void ActivateFlagIsSyncedInVinX(Component component)
    {
        _vinXConnector.SetIsSyncedToJiraToTrue(component);
    }
}