using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public class LinkAdminClient : RestClient, ILinkAdminClient
{
    private const string LinkAdminEndpoint = "LinkAdmin";

    public LinkAdminClient(IHttpConfig config, IRequestResponseLogger logger = null) : base(config, logger)
    {
    }

    public async Task<IEnumerable<LinkInfo>> SyncLinksAsync(IEnumerable<LinkData> links)
    {
        try
        {
            var requestBody = links.Select(l => new LinkRegistration
            {
                Name = l.Name,
                Url = l.Link,
                Description = l.Description
            });

            var request = new JsonRestRequest(HttpMethod.Post, LinkAdminEndpoint + "/Link/Sync");
            request.SetJsonBody(requestBody);
            var linkList = await PostAsync<List<LinkInfo>>(request);

            return linkList;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.SyncLinkListError, ex);
        }
    }
}