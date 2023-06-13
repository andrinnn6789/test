using System.Collections.Generic;

using IAG.Infrastructure.Rest.Authentication;

namespace IAG.Infrastructure.Rest.Atlas;

public class AtlasConfig : HttpConfig
{
    public AtlasConfig(AtlasCredentials credetials = null)
    {
        HttpHeaders = new Dictionary<string, string> { { "Accept", ContentTypes.ApplicationJson + ";" + MetaLevel.Compact.ToString().ToLower() } };
        if (credetials != null)
            Configure(credetials);
    }

    public void Configure(AtlasCredentials credentials)
    {
        BaseUrl = credentials.BaseUrl;
        if (string.IsNullOrWhiteSpace(credentials.User) || string.IsNullOrWhiteSpace(credentials.Password))
            Authentication = null;
        else
            Authentication = new BasicAuthentication
            {
                User = credentials.User,
                Password = credentials.Password
            };
    }
}