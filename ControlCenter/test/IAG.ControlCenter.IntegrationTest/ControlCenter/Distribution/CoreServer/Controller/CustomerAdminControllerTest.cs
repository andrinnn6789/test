using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Distribution.CoreServer.Controller;

[Collection("TestEnvironmentCollection")]
public class CustomerAdminControllerTest
{
    private const string Uri = ControlCenterEndpoints.Distribution + "CustomerAdmin/";
    private readonly HttpClient _client;

    public CustomerAdminControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task RegisterAndGetCustomersTest()
    {
        var customerId = Guid.NewGuid();
        var customerRegistration = new CustomerRegistration
        {
            CustomerId = customerId,
            CustomerName = "Alpha Company",
            CustomerCategoryId = 42,
            Description = "TestDescription"
        };
        var createCustomerResponse = await _client.PostAsync(Uri + "Customer", new StringContent(JsonConvert.SerializeObject(customerRegistration), Encoding.UTF8, "application/json"));
        createCustomerResponse.EnsureSuccessStatusCode();
        var customer = JsonConvert.DeserializeObject<CustomerInfo>(await createCustomerResponse.Content.ReadAsStringAsync());

        var getCustomersResponse = await _client.GetAsync(Uri + "Customer");
        getCustomersResponse.EnsureSuccessStatusCode();
        var customers = JsonConvert.DeserializeObject<List<CustomerInfo>>(await getCustomersResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, createCustomerResponse.StatusCode);
        Assert.NotNull(customer);
        Assert.Equal(customerId, customer.Id);
        Assert.Equal("Alpha Company", customer.CustomerName);
        Assert.Equal(42, customer.CustomerCategoryId);
        Assert.Equal("TestDescription", customer.Description);
        Assert.NotNull(customers);
        Assert.Equal(customer.Id, Assert.Single(customers)?.Id);
    }

    [Fact]
    public async Task RegisterCustomerFailuresTest()
    {
        var registrationEmpty = new CustomerRegistration();
        var registrationWithEmptyName = new CustomerRegistration { CustomerName = string.Empty };
        var registrationWithEmptyCustomerId = new CustomerRegistration { CustomerName = "NotEmpty", CustomerId = Guid.Empty};

        var responseEmpty = await _client.PostAsync(Uri + "Customer", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var responseEmptyRegistration = await _client.PostAsync(Uri + "Customer", new StringContent(JsonConvert.SerializeObject(registrationEmpty), Encoding.UTF8, "application/json"));
        var responseEmptyName = await _client.PostAsync(Uri + "Customer", new StringContent(JsonConvert.SerializeObject(registrationWithEmptyName), Encoding.UTF8, "application/json"));
        var responseEmptyCustomerId = await _client.PostAsync(Uri + "Customer", new StringContent(JsonConvert.SerializeObject(registrationWithEmptyCustomerId), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, responseEmpty.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyRegistration.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyName.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseEmptyCustomerId.StatusCode);
    }

    [Fact]
    public async Task AddAndRemoveProductsTest()
    {
        var productRegistration = new ProductRegistration { ProductName = "TestProduct" };
        var productRegistrationProduct = await _client.PostAsync(ControlCenterEndpoints.Distribution + "ProductAdmin/Product", new StringContent(JsonConvert.SerializeObject(productRegistration), Encoding.UTF8, "application/json"));
        productRegistrationProduct.EnsureSuccessStatusCode();
        var productId = JsonConvert.DeserializeObject<ProductInfo>(await productRegistrationProduct.Content.ReadAsStringAsync())?.Id;

        var customerRegistration = new CustomerRegistration { CustomerName = "Beta Company", CustomerId = Guid.NewGuid() };
        var createCustomerResponse = await _client.PostAsync(Uri + "Customer", new StringContent(JsonConvert.SerializeObject(customerRegistration), Encoding.UTF8, "application/json"));
        createCustomerResponse.EnsureSuccessStatusCode();
        var customerId = JsonConvert.DeserializeObject<CustomerInfo>(await createCustomerResponse.Content.ReadAsStringAsync())?.Id;

        var products = new[] { productId };
        var addProductResponse = await _client.PostAsync(Uri + $"Customer/{customerId}/AddProducts", new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json"));
        addProductResponse.EnsureSuccessStatusCode();

        var getCustomersAfterAddingProductResponse = await _client.GetAsync(Uri + "Customer");
        getCustomersAfterAddingProductResponse.EnsureSuccessStatusCode();
        var customerAfterAddingProduct = JsonConvert.DeserializeObject<List<CustomerInfo>>(await getCustomersAfterAddingProductResponse.Content.ReadAsStringAsync())?.FirstOrDefault(c => c.Id == customerId);

        var removeProductResponse = await _client.PostAsync(Uri + $"Customer/{customerId}/RemoveProducts", new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json"));
        removeProductResponse.EnsureSuccessStatusCode();

        var getCustomersAfterRemovingProductResponse = await _client.GetAsync(Uri + "Customer");
        getCustomersAfterRemovingProductResponse.EnsureSuccessStatusCode();
        var customerAfterRemovingProduct = JsonConvert.DeserializeObject<List<CustomerInfo>>(await getCustomersAfterRemovingProductResponse.Content.ReadAsStringAsync())?.FirstOrDefault(c => c.Id == customerId);

        Assert.NotNull(customerAfterAddingProduct);
        Assert.Equal(productId, Assert.Single(customerAfterAddingProduct.ProductIds));
        Assert.NotNull(customerAfterRemovingProduct);
        Assert.Empty(customerAfterRemovingProduct.ProductIds);
    }

    [Fact]
    public async Task AddAndRemoveProductsFailureTest()
    {
        var addProductWithEmptyCustomerResponse = await _client.PostAsync(Uri + $"Customer/{Guid.Empty}/AddProducts", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var addProductWithNullProductsResponse = await _client.PostAsync(Uri + $"Customer/{Guid.NewGuid()}/AddProducts", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var removeProductWithEmptyCustomerResponse = await _client.PostAsync(Uri + $"Customer/{Guid.Empty}/RemoveProducts", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        var removeProductWithNullProductsResponse = await _client.PostAsync(Uri + $"Customer/{Guid.NewGuid()}/RemoveProducts", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            
        Assert.Equal(HttpStatusCode.BadRequest, addProductWithEmptyCustomerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, addProductWithNullProductsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, removeProductWithEmptyCustomerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, removeProductWithNullProductsResponse.StatusCode);
    }
}