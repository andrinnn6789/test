using System.Net;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.PerformX.CampusSursee.Dto.Event;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

[Collection("CampusSurseeController")]
public class EventControllerTest : BaseControllerTest
{
    public EventControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Event")
    {
    }

    [Fact]
    public async Task EventGet()
    {
        var response = await Client.GetAsync($"{Url}Event(-1)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var firstEvent = await ChangedOnAndTake1Get<Event>();
        response = await Client.GetAsync($"{Url}Event({firstEvent.Id})");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task OccurenceGet()
    {
        await ChangedOnAndTake1Get<Occurence>();
    }

    [Fact]
    public async Task AdditionalGet()
    {
        await ChangedOnAndTake1Get<Additional>();
    }

    [Fact]
    public async Task EventModuleGet()
    {
        await ChangedOnAndTake1Get<EventModule>();
    }
}