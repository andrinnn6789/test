using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Lookup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

[Collection("CampusSurseeController")]
public class LookupControllerTest : BaseControllerTest
{
    public LookupControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Lookup")
    {
    }

    [Fact]
    public async Task SalutationGet()
    {
        await ChangedOnAndTake1Get<Salutation>();
    }

    [Fact]
    public async Task RegistrationStatusGet()
    {
        await ChangedOnAndTake1Get<RegistrationStatus>();
    }

    [Fact]
    public async Task EventStatusGet()
    {
        await ChangedOnAndTake1Get<EventStatus>();
    }

    [Fact]
    public async Task EventKindGet()
    {
        await ChangedOnAndTake1Get<EventKind>();
    }

    [Fact]
    public async Task AddressRelationKindGet()
    {
        await ChangedOnAndTake1Get<AddressRelationKind>();
    }

    [Fact]
    public async Task CommunicationChannelGet()
    {
        await ChangedOnAndTake1Get<CommunicationChannel>();
    }

    [Fact]
    public async Task CountryGet()
    {
        await ChangedOnAndTake1Get<Country>();
    }

    [Fact]
    public async Task DocumentGet()
    {
        var response = await Client.GetAsync($"{Uri}Address/Document?%24top=1");
        var doc = JsonConvert.DeserializeObject<List<Document>>(await response.Content.ReadAsStringAsync())?.First();
        response = await Client.GetAsync($"{Url}Document({doc?.Id})");
        response.EnsureSuccessStatusCode();
        var mediaData = response.Content.ReadAsByteArrayAsync().Result;

        Assert.NotNull(mediaData);
        Assert.NotNull(response.Content.Headers.ContentDisposition?.FileName);
        Assert.NotEmpty(response.Content.Headers.ContentDisposition?.FileName ?? throw new InvalidOperationException());
        response = await Client.GetAsync($"{Url}Document(-1)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}