using System;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.Jira;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.Jira;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.VinX;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.BusinessLogic;

public class WorklogSyncer : IWorklogSyncer
{
    private readonly IWorklogSyncVinXConnector _vinXConnector;
    
    private WorklogSyncJiraConnector _jiraConnector;
    private IMessageLogger _messageLogger;

    public WorklogSyncer(IWorklogSyncVinXConnector vinXConnector)
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
        _jiraConnector = new WorklogSyncJiraConnector(httpConfig, requestLogger);
        _messageLogger = messageLogger;
    }

    public IJobResult SyncWorklogs(DateTime lastSync)
    {
        var result = new JobResult();
        result.Result = JobResultEnum.Success;
        
        try
        {
            var workingHours = _vinXConnector.GetWorkingHours(lastSync).ToList();
            if (workingHours.Count == 0)
                return result;
            
            var employees = _vinXConnector.GetEmployees().ToList();
            var issues = _vinXConnector.GetPendenzen().ToList();

            var worklogIds = workingHours
                .Where(w => !string.IsNullOrEmpty(w.WorklogId))
                .Select(w => Convert.ToInt32(w.WorklogId)).ToArray();
            
            var worklogRequestModel = new GetWorklogsRequestModel() { Ids = worklogIds };
            var worklogs = _jiraConnector.GetWorklogs(worklogRequestModel).Result;
            
            foreach (var workingHour in workingHours)
            {
                var employeeFromWorkingHour = employees.First(e => e.ID == workingHour.MitarbeiterID);
                var pendenzFromWorkingHour = issues.FirstOrDefault(i => i.ID == workingHour.PendenzID);
                
                if(pendenzFromWorkingHour == null || workingHour.Datum < lastSync.Date.AddYears(-1))
                    // ignore issues older than 1 year in case they've been updated recently
                    continue;

                try
                {
                    var hasWorklogId = !string.IsNullOrEmpty(workingHour.WorklogId);
                    
                    if (hasWorklogId)
                    {
                        var issueFromJira = _jiraConnector.GetIssue(pendenzFromWorkingHour.JiraKey).Result;
                        var worklogFromWorkingHour = worklogs.First(w => w.Id == workingHour.WorklogId);

                        if (issueFromJira.Id == worklogFromWorkingHour.IssueId)
                        {
                            UpdateWorklog(workingHour, employeeFromWorkingHour, worklogFromWorkingHour);
                        }
                        else
                        {
                            DeleteWorklog(pendenzFromWorkingHour, workingHour);
                            
                            if(!pendenzFromWorkingHour.JiraKey.StartsWith("WA-"))
                                AddWorklogAndUpdateWorkingHour(workingHour, employeeFromWorkingHour, pendenzFromWorkingHour);
                        }
                    }
                    else
                    {
                        if(!pendenzFromWorkingHour.JiraKey.StartsWith("WA-"))
                            AddWorklogAndUpdateWorkingHour(workingHour, employeeFromWorkingHour, pendenzFromWorkingHour);
                    }
                }
                catch (Exception ex)
                {
                    _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.FailedWorklogProcessingErrorFormatMessage, 
                        workingHour.ID, employeeFromWorkingHour.Login, LocalizableException.GetExceptionMessage(ex));
                    _messageLogger.AddMessage(ex);
                    
                    result.ErrorCount++;
                    result.Result = JobResultEnum.PartialSuccess;
                }
            }
        }
        catch (Exception ex)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.LoadJiraWorklogsErrorFormatMessage,  LocalizableException.GetExceptionMessage(ex));
            _messageLogger.AddMessage(ex);

            result.ErrorCount++;
            result.Result = JobResultEnum.Failed;
            
            _vinXConnector.Dispose();
            _jiraConnector.Dispose();
        }

        return result;
    }

    private void DeleteWorklog(Pendenz pendenzFromWorkingHour, Zeit workingHour)
    {
        _ = _jiraConnector.DeleteWorklog(pendenzFromWorkingHour.JiraKey, workingHour.WorklogId);
    }

    private void UpdateWorklog(Zeit workingHour, Mitarbeiter employeeFromWorkingHour, Worklog worklogFromWorkingHour)
    {
        var request = CreateAddUpdateWorklogRequest(workingHour, employeeFromWorkingHour);
        _ = _jiraConnector.UpdateWorklog(worklogFromWorkingHour.IssueId, worklogFromWorkingHour.Id, request);
    }

    private void AddWorklogAndUpdateWorkingHour(Zeit workingHour, Mitarbeiter employeeFromWorkingHour,
        Pendenz pendenzFromWorkingHour)
    {
        var request = CreateAddUpdateWorklogRequest(workingHour, employeeFromWorkingHour);
        var returnedWorklog = _jiraConnector.CreateWorklog(pendenzFromWorkingHour.JiraKey, request).Result;

        workingHour.WorklogId = returnedWorklog.Id;
        _vinXConnector.UpdateWorkingHour(workingHour);
    }

    private AddUpdateWorklogRequestModel CreateAddUpdateWorklogRequest(Zeit workingHour, Mitarbeiter employee)
    {
        var timeSpent = workingHour.BisZeit - workingHour.VonZeit ?? TimeSpan.Zero;

        var addUpdateWorklogRequest = new AddUpdateWorklogRequestModel()
        {
            Comment = $"[{employee.Login}] {workingHour.Problem}",
            TimeSpentSeconds = Convert.ToInt32(timeSpent.TotalSeconds),
            Started = workingHour.Datum.ToString("yyyy-MM-ddThh:mm:ss.mszz00")
        };

        return addUpdateWorklogRequest;
    }
}