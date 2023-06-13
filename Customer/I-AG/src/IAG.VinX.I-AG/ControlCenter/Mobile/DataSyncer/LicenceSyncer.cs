using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Mobile.DataSyncer;

public class LicenceSyncer
{
    private readonly IVinXConnectorIag _vinxConnector;
    private readonly IHttpConfig _controlCenterConfig;
    private readonly ILogger _logger;
    private readonly DateTime _lastUpload;
    private readonly bool _doFullSync;

    public LicenceSyncer(IVinXConnectorIag vinXConnector, IHttpConfig controlCenterConfig, ILogger logger, DateTime lastUpload, bool doFullSync)
    {
        _vinxConnector = vinXConnector;
        _controlCenterConfig = controlCenterConfig;
        _logger = logger;
        _lastUpload = lastUpload;
        _doFullSync = doFullSync;
    }

    public async Task<int> Sync()
    {
        var licenceConfigs = _vinxConnector.GetLicenceConfig(_doFullSync ? DateTime.MinValue : _lastUpload);
        if (_doFullSync)
        {
            var backend = BuildRequest(licenceConfigs, out var request, "Sync");
            var licResponse = await backend.PostAsync<List<MobileLicence>>(request);
            _vinxConnector.BeginTransaction();
            try
            {
                foreach (var licence in licResponse)
                {
                    _vinxConnector.UpdateLicence(licence);
                }

                _vinxConnector.Commit();
            }
            catch
            {
                _vinxConnector.Rollback();
                throw;
            }

            return licResponse.Count;
        }
        else
        {
            if (licenceConfigs.Licences.Count == 0 && licenceConfigs.Tenants.Count == 0 && licenceConfigs.Installations.Count == 0)
                return 0;
            var backend = BuildRequest(licenceConfigs, out var request, "Update");
            await backend.PostAsync<List<MobileLicence>>(request);
            return licenceConfigs.Licences.Count;
        }
    }

    private RestClient BuildRequest(LicenceSync licenceConfigs, out JsonRestRequest request, string method)
    {
        var backend = new RestClient(_controlCenterConfig, new RequestResponseLogger(_logger));
        request = new JsonRestRequest(HttpMethod.Post, method);
        request.SetJsonBody(licenceConfigs);
        return backend;
    }
}