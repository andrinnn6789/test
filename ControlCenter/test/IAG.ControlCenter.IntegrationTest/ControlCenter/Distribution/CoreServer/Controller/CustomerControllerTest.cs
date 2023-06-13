using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Distribution.CoreServer.Controller;

[Collection("TestEnvironmentCollection")]
public class CustomerControllerTest
{
    private const string Uri = ControlCenterEndpoints.Distribution + "Customer/";
    private readonly HttpClient _client;
    private readonly DistributionDbContext _dbContext;
    private readonly Guid _testProductId = Guid.NewGuid();
    private readonly Guid _testReleaseId = Guid.NewGuid();
    private readonly Guid _testCustomerAlphaId = Guid.NewGuid();
    private readonly Guid _testCustomerBetaId = Guid.NewGuid();
    private readonly Guid _testFileId = Guid.NewGuid();
    private readonly Guid _testLinkId = Guid.NewGuid();


    public CustomerControllerTest(TestServerEnvironment testEnvironment)
    {
        _dbContext = testEnvironment.GetServices().GetRequiredService<DistributionDbContext>();

        _dbContext.Products.Add(new Product { Id = _testProductId, Name = "TestProduct" });
        _dbContext.Releases.Add(new Release { Id = _testReleaseId, ProductId = _testProductId, ReleaseVersion = "1.0", Platform = "Win", ReleaseDate = DateTime.Today.AddDays(-1)});
        _dbContext.Customers.Add(new Customer { Id = _testCustomerAlphaId, Name = "Alpha Company" });
        _dbContext.Customers.Add(new Customer { Id = _testCustomerBetaId, Name = "Beta Company" });
        _dbContext.ProductCustomers.Add(new ProductCustomer { ProductId = _testProductId, CustomerId = _testCustomerAlphaId });
        _dbContext.FileStores.Add(new FileStore { Id = _testFileId, Name = "Base.dll", Data = Encoding.UTF8.GetBytes("TestContent") });
        _dbContext.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = _testReleaseId, FileStoreId = _testFileId });

        _dbContext.SaveChanges();

        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task GetCustomerTest()
    {
        var getCustomerResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}");
        getCustomerResponse.EnsureSuccessStatusCode();
        var customer = JsonConvert.DeserializeObject<CustomerInfo>(await getCustomerResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}");

        Assert.NotNull(customer);
        Assert.Equal(_testCustomerAlphaId, customer.Id);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var getProductsResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Product");
        getProductsResponse.EnsureSuccessStatusCode();
        var products = JsonConvert.DeserializeObject<List<ProductInfo>>(await getProductsResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}/Product");

        Assert.NotNull(products);
        Assert.Equal(_testProductId, Assert.Single(products)?.Id);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var getReleasesResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Product/{_testProductId}/Release");
        getReleasesResponse.EnsureSuccessStatusCode();
        var releases = JsonConvert.DeserializeObject<List<ReleaseInfo>>(await getReleasesResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}/Product/{_testProductId}/Release");
        var responseUnknownProduct = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Product/{Guid.NewGuid()}/Release");
        var responseNotAssignedProduct = await _client.GetAsync(Uri + $"{_testCustomerBetaId}/Product/{_testProductId}/Release");

        Assert.NotNull(releases);
        Assert.Equal(_testReleaseId, Assert.Single(releases)?.Id);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownProduct.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseNotAssignedProduct.StatusCode);
    }

    [Fact]
    public async Task GetReleaseFilesTest()
    {
        var getReleaseFilesResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Product/{_testProductId}/Release/{_testReleaseId}/File");
        getReleaseFilesResponse.EnsureSuccessStatusCode();
        var files = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await getReleaseFilesResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}/Product/{_testProductId}/Release/{_testReleaseId}/File");
        var responseUnknownRelease = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Product/{_testProductId}/Release/{Guid.NewGuid()}/File");
        var responseNotAssignedRelease = await _client.GetAsync(Uri + $"{_testCustomerBetaId}/Product/{_testProductId}/Release/{_testReleaseId}/File");

        Assert.NotNull(files);
        Assert.Equal(_testFileId, Assert.Single(files)?.Id);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownRelease.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseNotAssignedRelease.StatusCode);
    }

    [Fact]
    public async Task GetFileContentTest()
    {
        var getFileContentResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/File/{_testFileId}");
        getFileContentResponse.EnsureSuccessStatusCode();
        var content = await getFileContentResponse.Content.ReadAsByteArrayAsync();

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}/File/{_testFileId}");
        var responseUnknownFile = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/File/{Guid.NewGuid()}");
        var responseNotAssignedFile = await _client.GetAsync(Uri + $"{_testCustomerBetaId}/File/{_testFileId}");

        Assert.Equal(Encoding.UTF8.GetBytes("TestContent"), content);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownFile.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseNotAssignedFile.StatusCode);
    }

    [Fact]
    public async Task RegisterInstallationTest()
    {
        var testInstallation = new InstallationRegistration
        {
            InstanceName = "TestInstallation",
            ReleaseVersion = "TestPath",
            Description = "TestComment",
            Platform = "win-x64",
            ProductId = _testProductId
        };
        var transferInstallation = new InstallationRegistration
        {
            InstanceName = "TransferInstallation",
            ReleaseVersion = "TransferPath",
            Platform = "win-x64",
            Description = "Transferred",
            ProductId = _testProductId
        };
        var testUnInstallation = new InstallationRegistration
        {
            InstanceName = "TestInstallation",
            ReleaseVersion = "TestPath",
            Description = "Uninstalled",
            Platform = "win-x64",
            ProductId = _testProductId
        };
        var noNameInstallation = new InstallationRegistration
        {
            ReleaseVersion = "TestPath",
            ProductId = _testProductId
        };
        var noPathInstallation = new InstallationRegistration
        {
            InstanceName = "TestInstallation",
            ProductId = _testProductId
        };

        var installResponse = await _client.PostAsync(Uri + $"{_testCustomerAlphaId}/Installation", new StringContent(JsonConvert.SerializeObject(testInstallation), Encoding.UTF8, "application/json"));
        installResponse.EnsureSuccessStatusCode();
        var installation = JsonConvert.DeserializeObject<InstallationInfo>(await installResponse.Content.ReadAsStringAsync());

        var transferResponse = await _client.PostAsync(
            Uri + $"{_testCustomerAlphaId}/Installation", 
            new StringContent(JsonConvert.SerializeObject(transferInstallation), Encoding.UTF8, "application/json"));
        transferResponse.EnsureSuccessStatusCode();
        var transfer = JsonConvert.DeserializeObject<InstallationInfo>(await transferResponse.Content.ReadAsStringAsync());

        var unInstallResponse = await _client.PostAsync(
            Uri + $"{_testCustomerAlphaId}/Installation", 
            new StringContent(JsonConvert.SerializeObject(testUnInstallation), Encoding.UTF8, "application/json"));
        unInstallResponse.EnsureSuccessStatusCode();
        var unInstallation = JsonConvert.DeserializeObject<InstallationInfo>(await unInstallResponse.Content.ReadAsStringAsync());

        var responseNoPathInstallationResponse = await _client.PostAsync(
            Uri + $"{_testCustomerAlphaId}/Installation",
            new StringContent(JsonConvert.SerializeObject(noPathInstallation), Encoding.UTF8, "application/json"));
        responseNoPathInstallationResponse.EnsureSuccessStatusCode();
        var noPathInstallationResult = JsonConvert.DeserializeObject<InstallationInfo>(await responseNoPathInstallationResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.PostAsync(
            Uri + $"{Guid.NewGuid()}/Installation", 
            new StringContent(JsonConvert.SerializeObject(testInstallation), Encoding.UTF8, "application/json"));
        var responseNoNameInstallation = await _client.PostAsync(
            Uri + $"{_testCustomerAlphaId}/Installation", 
            new StringContent(JsonConvert.SerializeObject(noNameInstallation), Encoding.UTF8, "application/json"));

        Assert.NotNull(installation);
        Assert.NotNull(transfer);
        Assert.NotNull(unInstallation);
        Assert.NotNull(noPathInstallationResult);
        Assert.NotEqual(installation.Id, unInstallation.Id);
        Assert.NotEqual(installation.Id, transfer.Id);
        Assert.NotEqual(transfer.Id, unInstallation.Id);
        Assert.Equal(_testCustomerAlphaId, installation.CustomerId);
        Assert.Equal(_testCustomerAlphaId, transfer.CustomerId);
        Assert.Equal(_testCustomerAlphaId, unInstallation.CustomerId);
        Assert.Equal(_testCustomerAlphaId, noPathInstallationResult.CustomerId);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseNoNameInstallation.StatusCode);
    }

    [Fact]
    public async Task GetLinksTest()
    {
        _dbContext.Links.Add(new Link { Id = _testLinkId, Name = "TestLink", Url = "https://www.i-ag.ch" });
        await _dbContext.SaveChangesAsync();

        var getLinksResponse = await _client.GetAsync(Uri + $"{_testCustomerAlphaId}/Link");
        getLinksResponse.EnsureSuccessStatusCode();
        var links = JsonConvert.DeserializeObject<List<LinkInfo>>(await getLinksResponse.Content.ReadAsStringAsync());

        var responseUnknownCustomer = await _client.GetAsync(Uri + $"{Guid.NewGuid()}/Link");

        Assert.NotNull(links);
        Assert.Equal(_testLinkId, Assert.Single(links)?.Id);
        Assert.Equal(HttpStatusCode.Unauthorized, responseUnknownCustomer.StatusCode);
    }
}