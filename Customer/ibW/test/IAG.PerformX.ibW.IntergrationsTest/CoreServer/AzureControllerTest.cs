using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.ibW.Dto.Azure;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.CoreServer;

[TestCaseOrderer("IAG.Infrastructure.Test.xUnit.PriorityOrderer", "IAG.Infrastructure.Test")]
public class AzureControllerTest : BaseControllerTest
{
    public AzureControllerTest(TestServerEnvironment testEnvironment): 
        base(testEnvironment, "api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/Azure/")
    {
    }

    [Fact]
    public async Task RequestTokenTest()
    {
        var item = RequestTokenParameter.ForPasswordGrant("test", "test").ForRealm("test");

        var response = await Client.PostAsync($"{EndPoint}RequestToken",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminUnitGet()
    {
        await Take1Get<AdminUnit>();
    }

    [Fact, TestPriority(1)]
    public async Task PostPersonChangeLogin()
    {
        var person = await ChangedOnAndTake1Get<Person>();
        var personParamOri = new PersonChangeParam
        {
            CloudEMail = person.CloudEMail,
            CloudLogin = person.CloudLogin
        };
        var item = new PersonChangeParam
        {
            CloudEMail = "test@test.test",
            CloudLogin = "test.test"
        };
        try
        {
            var response = await Client.PostAsync($"{EndPoint}{nameof(Person)}({person.Id})/SetCloudLogin",
                new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await Client.PostAsync($"{EndPoint}{nameof(Person)}(-1)/SetCloudLogin",
                new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            response = await Client.PostAsync($"{EndPoint}{nameof(Person)}({person.Id})/SetCloudLogin",
                new StringContent(JsonConvert.SerializeObject(new PersonChangeParam()), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await Client.PostAsync($"{EndPoint}{nameof(Person)}({person.Id})/SetCloudLogin",
                new StringContent(JsonConvert.SerializeObject(personParamOri), Encoding.UTF8, "application/json"));
        }
    }

    [Fact, TestPriority(2)]
    public async Task PostPersonResetLogin()
    {
        var person = await ChangedOnAndTake1Get<Person>();
        var personParamOri = new PersonChangeParam
        {
            CloudEMail = person.CloudEMail,
            CloudLogin = person.CloudLogin
        };
        try
        {
            var response = await Client.PostAsync($"{EndPoint}{nameof(Person)}({person.Id})/ResetCloudLogin",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await Client.PostAsync($"{EndPoint}{nameof(Person)}(-1)/ResetCloudLogin",
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await Client.PostAsync($"{EndPoint}{nameof(Person)}({person.Id})/SetCloudLogin",
                new StringContent(JsonConvert.SerializeObject(personParamOri), Encoding.UTF8, "application/json"));
        }
    }

    [Fact]
    public async Task GroupGet()
    {
        var group = await ChangedOnAndTake1Get<Group>();
        var response = await Client.GetAsync($"{EndPoint}Group({group.Id})");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response = await Client.GetAsync($"{EndPoint}Group(-1)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GroupRelationGet()
    {
        var groupRelation = await Take1Get<GroupRelation>();
        var response = await Client.GetAsync($"{EndPoint}GroupRelation({groupRelation.Id})");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response = await Client.GetAsync($"{EndPoint}GroupRelation(-1)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task OwnerGet()
    {
        await ChangedOnAndTake1Get<Owner>();
    }

    [Fact]
    public async Task MemberGet()
    {
        await ChangedOnAndTake1Get<Member>();
    }
}