using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;
using IAG.PerformX.HGf.Resource;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

[JobInfo("DAB29ED0-CC20-45F9-A164-06B0144B153F", JobName)]
public class RegistrationsJob : JobBase<RegistrationsConfig, JobParameter, RegistrationsResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "Registrations";

    private readonly IRequestResponseLogger _requestLogger;
    private RegistrationMapper _registrationMapper;
    private RegistrationResultMapper _registrationResultMapper;
    private RegistrationRequestClient _registrationRequestClient;
    private RegistrationResultClient _registrationResultClient;
    private SaveRegistrationsClient _saveRegistrationsClient;

    public RegistrationsJob(ILogger<RegistrationsJob> logger)
    {
        _requestLogger = new RequestResponseLogger(logger);
    }

    protected override void ExecuteJob()
    {
        _registrationMapper = new RegistrationMapper(Config.AtlasBasePath);
        _registrationResultMapper = new RegistrationResultMapper();
        _registrationRequestClient = new RegistrationRequestClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _registrationResultClient = new RegistrationResultClient(new AtlasConfig(Config.AtlasCredentials), _requestLogger);
        _saveRegistrationsClient = new SaveRegistrationsClient(Config.LgavConfig, _requestLogger);

        var atlasRegistrations = GetAtlasRegistrations().ToList();
        HeartbeatAndCheckCancellation();

        var lgavRegistrationList = MapRegistrations(atlasRegistrations);
        HeartbeatAndCheckCancellation();

        if (lgavRegistrationList.RegistrationList.RegistrationListItems.Count > 0)
        {
            var lgavResults = SaveRegistrations(lgavRegistrationList);
            Heartbeat();
            WriteResults(lgavResults);
        }

        if (Result.ErrorCount > 0)
        {
            Result.Result = Result.SuccessfulWriteResultCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private IEnumerable<AtlasRegistration> GetAtlasRegistrations()
    {
        List<AtlasRegistration> atlasRegistrations;
        try
        {
            atlasRegistrations = _registrationRequestClient.Get().Result.ToList();
            Result.AtlasRegistrationsCount = atlasRegistrations.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RegistrationsGetFromAtlasErrorFormatMessage, ex);
        }

        return atlasRegistrations;
    }

    private RegistrationListMainObject MapRegistrations(IEnumerable<AtlasRegistration> atlasRegistrations)
    {
        var lgavRegistrationList = new RegistrationListMainObject();
        lgavRegistrationList.RegistrationList.Header.ApiKey = Config.LgavApiKey;
        lgavRegistrationList.RegistrationList.Header.TransactionDate = DateTime.Now.ToString("s");

        foreach (var atlasRegistration in atlasRegistrations)
        {
            try
            {
                var lgavRegistration = _registrationMapper.NewDestination(atlasRegistration);
                lgavRegistrationList.RegistrationList.RegistrationListItems.Add(lgavRegistration);
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.RegistrationsMapToLgavErrorFormatMessage, JsonConvert.SerializeObject(atlasRegistration));
                AddMessage(ex);
            }
        }

        return lgavRegistrationList;
    }

    private RegistrationListResponseMainObject SaveRegistrations(RegistrationListMainObject lgavRegistrationList)
    {
        RegistrationListResponseMainObject lgavResults;
        try
        {
            lgavResults = _saveRegistrationsClient.Post(lgavRegistrationList).Result;
            Result.LgavResultRegistrationsCount = lgavResults.RegistrationListResponseItems.Count;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RegistrationsWriteToLgavErrorFormatMessage, ex);
        }

        return lgavResults;
    }

    private void WriteResults(RegistrationListResponseMainObject lgavResults)
    {
        foreach (var lgavResult in lgavResults.RegistrationListResponseItems)
        {
            var atlasResult = _registrationResultMapper.NewDestination(lgavResult);

            try
            {
                _registrationResultClient.Put(atlasResult, lgavResult.RegistrationUid).Wait();
                Result.SuccessfulWriteResultCount++;
                Heartbeat();
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.RegistrationsWriteResultErrorFormatMessage, JsonConvert.SerializeObject(lgavResult));
                AddMessage(ex);
                Result.ErrorCount++;
            }
        }
    }
}