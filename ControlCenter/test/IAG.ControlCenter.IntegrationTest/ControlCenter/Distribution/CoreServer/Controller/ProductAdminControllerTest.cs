using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Distribution.CoreServer.Controller;

[Collection("TestEnvironmentCollection")]
public class ProductAdminControllerTest
{
    private const string Uri = ControlCenterEndpoints.Distribution + "ProductAdmin/";
    private readonly HttpClient _client;

    public ProductAdminControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var response = await _client.GetAsync(Uri + "Product");
        response.EnsureSuccessStatusCode();
        var products = JsonConvert.DeserializeObject<List<ProductInfo>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(products);
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var response = await _client.GetAsync(Uri + "Release");
        response.EnsureSuccessStatusCode();
        var releases = JsonConvert.DeserializeObject<List<ReleaseInfo>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(releases);
    }

    [Fact]
    public async Task RegisterProductTest()
    {
        var productRegistration = new ProductRegistration
        {
            ProductName = "SimpleTestProduct",
            Description = "TestProductDescription",
            Type = ProductType.IagService,
        };
        var productResponse = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        productResponse.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await productResponse.Content.ReadAsStringAsync());

        var pluginRegistration = new ProductRegistration
        {
            ProductName = "SimpleTestPlugin",
            Description = "TestPluginDescription",
            Type = ProductType.CustomerExtension,
            DependsOnProductId = product?.Id
        };
        var pluginResponse = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(pluginRegistration), Encoding.UTF8, "application/json"));
        pluginResponse.EnsureSuccessStatusCode();
        var plugin = JsonConvert.DeserializeObject<ProductInfo>(await pluginResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, pluginResponse.StatusCode);
        Assert.NotNull(product);
        Assert.NotNull(plugin);
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.NotEqual(Guid.Empty, plugin.Id);
        Assert.Equal("SimpleTestProduct", product.ProductName);
        Assert.Equal("SimpleTestPlugin", plugin.ProductName);
        Assert.Equal("TestProductDescription", product.Description);
        Assert.Equal("TestPluginDescription", plugin.Description);
        Assert.Equal(ProductType.IagService, product.ProductType);
        Assert.Equal(ProductType.CustomerExtension, plugin.ProductType);
        Assert.Null(product.DependsOnProductId);
        Assert.Equal(product.Id, plugin.DependsOnProductId);
    }

    [Fact]
    public async Task RegisterProductFailuresTest()
    {
        var registrationEmpty = new ProductRegistration();
        var registrationWithEmptyName = new ProductRegistration { ProductName = string.Empty };

        var responseEmpty = await _client.PostAsync(Uri + "Product", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var responseEmptyRegistration = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(registrationEmpty), Encoding.UTF8, "application/json"));
        var responseEmptyName = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(registrationWithEmptyName), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, responseEmpty.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyRegistration.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyName.StatusCode);
    }

    [Fact]
    public async Task RegisterReleaseTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());

        var releaseRegistration = new ReleaseRegistration
        {
            ReleaseVersion = "1.0.1",
            Platform = "win-x64",
            Description = "TestDescription",
            ReleasePath = "./TestPath/"
        };
        var registrationEmpty = new ReleaseRegistration();
        var registrationWithEmptyVersion = new ReleaseRegistration { ReleaseVersion = string.Empty };

        Assert.NotNull(product);
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var release = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());

        var responseEmpty = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var responseEmptyRegistration = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(registrationEmpty), Encoding.UTF8, "application/json"));
        var responseEmptyVersion = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(registrationWithEmptyVersion), Encoding.UTF8, "application/json"));
        var responseUnknownRelease = await _client.PostAsync(Uri + $"Product/{Guid.NewGuid()}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(release);
        Assert.NotEqual(Guid.Empty, release.Id);
        Assert.Equal(product.Id, release.ProductId);
        Assert.Equal("1.0.1", release.ReleaseVersion);
        Assert.Equal("TestDescription", release.Description);
        Assert.Equal("./TestPath/", release.ReleasePath);
        Assert.Null(release.ReleaseDate);

        Assert.Equal(HttpStatusCode.BadRequest, responseEmpty.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyRegistration.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyVersion.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responseUnknownRelease.StatusCode);
    }

    [Fact]
    public async Task AddFilesToReleaseTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(product);
        var releaseRegistration = new ReleaseRegistration { ReleaseVersion = "1.0.3", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var release = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());

        var fileDate = DateTime.UtcNow.AddMinutes(-1);
        var fileRegistrations = new List<FileRegistration>
        {
            new()
            {
                Name = "Test.dll",
                ProductVersion = "1.0.0",
                FileVersion = "1.0.0.1",
                Checksum = new byte[] {1, 2, 3},
                FileLastModifiedDate = fileDate
            },
            new()
            {
                Name = "Image.jpg",
                Checksum = new byte[] {4, 5, 6},
                FileLastModifiedDate = fileDate
            }
        };

        Assert.NotNull(release);
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/AddFiles", new StringContent(JsonConvert.SerializeObject(fileRegistrations), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var addedFiles = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await response.Content.ReadAsStringAsync());
        var fileTestDll = addedFiles?.FirstOrDefault(f => f.Name == "Test.dll");
        var fileImageDll = addedFiles?.FirstOrDefault(f => f.Name == "Image.jpg");

        var responseUnknownRelease = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{Guid.NewGuid()}/AddFiles", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(addedFiles);
        Assert.NotNull(fileTestDll);
        Assert.NotNull(fileImageDll);
        Assert.Equal(2, addedFiles.Count);
        Assert.Equal("1.0.0", fileTestDll.ProductVersion);
        Assert.Equal("1.0.0.1", fileTestDll.FileVersion);
        Assert.Equal(new byte[] { 1, 2, 3 }, fileTestDll.Checksum);
        Assert.Equal(new byte[] { 4, 5, 6 }, fileImageDll.Checksum);
        Assert.Equal(fileDate, fileTestDll.FileLastModifiedDate);
        Assert.Equal(fileDate, fileImageDll.FileLastModifiedDate);

        Assert.Equal(HttpStatusCode.NotFound, responseUnknownRelease.StatusCode);
    }

    [Fact]
    public async Task SetFileContentTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(product);

        var releaseRegistration = new ReleaseRegistration { ReleaseVersion = "1.0.4", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var release = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());

        var fileRegistrations = new List<FileRegistration>
        {
            new() { Name = "Test.dll", ProductVersion = "1.1.0", FileVersion = "1.1.0.1", Checksum = new byte[] {4, 5, 6} },
        };

        Assert.NotNull(release);
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/AddFiles", new StringContent(JsonConvert.SerializeObject(fileRegistrations), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var addedFile = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await response.Content.ReadAsStringAsync())?.First();
        Assert.NotNull(addedFile);

        var fileContent = new MultipartFormDataContent
        {
            { new ByteArrayContent(Encoding.UTF8.GetBytes("TestContent")), "file", "DoesNotMatter.foo" }
        };
        response = await _client.PostAsync(Uri + $"File/{addedFile.Id}", fileContent);
        response.EnsureSuccessStatusCode();
        var fileAfterSetContent = JsonConvert.DeserializeObject<FileMetaInfo>(await response.Content.ReadAsStringAsync());

        var responseUnknownFile = await _client.PostAsync(Uri + $"File/{Guid.NewGuid()}", fileContent);
        var responseAlreadySet = await _client.PostAsync(Uri + $"File/{addedFile.Id}", fileContent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(fileAfterSetContent);
        Assert.NotNull(fileAfterSetContent.Checksum);

        Assert.Equal(HttpStatusCode.NotFound, responseUnknownFile.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseAlreadySet.StatusCode);
    }

    [Fact]
    public async Task ApproveReleaseTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(product);

        var releaseRegistration = new ReleaseRegistration { ReleaseVersion = "1.0.2", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var release = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(release);

        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/Approve", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var approvedRelease = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());

        var responseUnknownRelease = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{Guid.NewGuid()}/Approve", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var responseSecondApprove = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/Approve", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var responseAddFilesAfterApproving = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/AddFiles", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(approvedRelease);
        Assert.Equal(release.Id, approvedRelease.Id);
        Assert.Equal(product.Id, approvedRelease.ProductId);
        Assert.Equal("1.0.2", release.ReleaseVersion);
        Assert.NotNull(approvedRelease.ReleaseDate);

        Assert.Equal(HttpStatusCode.NotFound, responseUnknownRelease.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseSecondApprove.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseAddFilesAfterApproving.StatusCode);
    }

    [Fact]
    public async Task RemoveReleaseTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(product);

        var releaseRegistration = new ReleaseRegistration { ReleaseVersion = "1.0.2", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var release = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(release);

        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{release.Id}/Remove", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseUnknownRelease = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{Guid.NewGuid()}/Remove", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responseUnknownRelease.StatusCode);
    }

    [Fact]
    public async Task OverallProcessTest()
    {
        // register new product
        var productRegistration = new ProductRegistration { ProductName = "NewTestProduct" };
        var response = await _client.PostAsync(Uri + "Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var product = JsonConvert.DeserializeObject<ProductInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(product);

        // register release 2.0
        var releaseRegistrationR20 = new ReleaseRegistration { ReleaseVersion = "2.0.0", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistrationR20), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var releaseR20 = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(releaseR20);

        // add files to release 2.0
        var fileRegistrationsR20 = new List<FileRegistration>
        {
            new() { Name = "BusinessLogic.dll", ProductVersion = "2.0.0", FileVersion = "2.0.0.1", Checksum = new byte[] { 20, 21, 22 } },
            new() { Name = "Logo.png", Checksum = new byte[] {13, 14, 15} },
        };

        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR20.Id}/AddFiles", new StringContent(JsonConvert.SerializeObject(fileRegistrationsR20), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var addedFilesR2 = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(addedFilesR2);
        var dllFileR20 = addedFilesR2.First(f => f.Name == "BusinessLogic.dll");
        var logoFileR20 = addedFilesR2.First(f => f.Name == "Logo.png");

        var fileContent = new MultipartFormDataContent { { new ByteArrayContent(Encoding.UTF8.GetBytes("TestContent")), "file", "DoesNotMatter.foo" } };
        (await _client.PostAsync(Uri + $"File/{dllFileR20.Id}", fileContent)).EnsureSuccessStatusCode();
            
        response = await _client.PostAsync(Uri + $"File/{logoFileR20.Id}", fileContent);
        response.EnsureSuccessStatusCode();
        logoFileR20 = JsonConvert.DeserializeObject<FileMetaInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(logoFileR20);

        // approve release 2.0
        (await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR20.Id}/Approve", new StringContent(string.Empty, Encoding.UTF8, "application/json"))).EnsureSuccessStatusCode();

        // register release 2.1
        var releaseRegistrationR21 = new ReleaseRegistration { ReleaseVersion = "2.1.0", Platform = "win-x64" };
        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release", new StringContent(JsonConvert.SerializeObject(releaseRegistrationR21), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var releaseR21 = JsonConvert.DeserializeObject<ReleaseInfo>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(releaseR21);

        // add files to release 2.1
        var fileRegistrationsR21 = new List<FileRegistration>
        {
            new() { Name = "BusinessLogic.dll", ProductVersion = "2.1.0", FileVersion = "2.1.0.1", Checksum = new byte[] { 21, 22, 23 } },
            new() { Name = logoFileR20.Name, Checksum = logoFileR20.Checksum },
        };

        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR21.Id}/AddFiles", new StringContent(JsonConvert.SerializeObject(fileRegistrationsR21), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var addedFilesR21 = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(addedFilesR21);
        var dllFileR21 = addedFilesR21.Single();

        // set content of "BusinessLogic.dll"
        (await _client.PostAsync(Uri + $"File/{dllFileR21?.Id}", fileContent)).EnsureSuccessStatusCode();

        // add more files to release 2.1
        fileRegistrationsR21 = new List<FileRegistration>
        {
            new() { Name = "ReadMe.txt", Checksum = new byte[] {14, 15, 16} },
        };

        response = await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR21.Id}/AddFiles", new StringContent(JsonConvert.SerializeObject(fileRegistrationsR21), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        addedFilesR21 = JsonConvert.DeserializeObject<List<FileMetaInfo>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(addedFilesR21);
        var readMeR21 = addedFilesR21.First(f => f.Name == "ReadMe.txt");

        // set content of "ReadMe.txt"
        (await _client.PostAsync(Uri + $"File/{readMeR21.Id}", fileContent)).EnsureSuccessStatusCode();

        // approve release 2.1
        (await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR21.Id}/Approve", new StringContent(string.Empty, Encoding.UTF8, "application/json"))).EnsureSuccessStatusCode();

        // remove release 2.0
        (await _client.PostAsync(Uri + $"Product/{product.Id}/Release/{releaseR20.Id}/Remove", new StringContent(string.Empty, Encoding.UTF8, "application/json"))).EnsureSuccessStatusCode();
    }
}