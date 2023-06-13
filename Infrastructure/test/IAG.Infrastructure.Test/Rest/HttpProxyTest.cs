using System;
using System.Net;

using IAG.Infrastructure.Rest;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class HttpProxyTest
{
    [Fact]
    public void ProxyTest()
    {
        var proxyUrl = "localhost:8888";
        var testUri = new Uri("http://test.i-ag.ch");
        var testCredentials = new NetworkCredential("test", "topsecret");
        var proxy = new HttpProxy(proxyUrl);
        proxy.Credentials = testCredentials;

        Assert.NotNull(proxy);
        Assert.NotNull(proxy.GetProxy(testUri));
        Assert.Equal(proxyUrl, proxy.GetProxy(testUri).ToString());
        Assert.False(proxy.IsBypassed(testUri));
        Assert.Equal(testCredentials, proxy.Credentials);
    }
}