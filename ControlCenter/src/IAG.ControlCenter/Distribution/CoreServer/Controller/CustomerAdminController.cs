using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Mvc;


namespace IAG.ControlCenter.Distribution.CoreServer.Controller;

[Route(ControlCenterEndpoints.Distribution + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
public class CustomerAdminController : ControllerBase
{
    private readonly ICustomerAdminHandler _handler;

    public CustomerAdminController(ICustomerAdminHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Registers a new customer.
    /// </summary>
    /// <remarks>
    /// This endpoint is used to register a new customer based on the provided customer registration data.
    /// It creates a new customer using the provided information and returns the customer information.
    /// </remarks>
    /// <param name="customerRequest">The customer registration data.</param>
    /// <returns>An action result containing the customer information.</returns>
    /// <response code="201">Indicates a successful customer registration and returns the created customer information.</response>
    /// <response code="400">If the customer registration data is null or invalid.</response>
    [HttpPost("Customer")]
    public async Task<ActionResult<CustomerInfo>> RegisterCustomer([FromBody] CustomerRegistration customerRequest)
    {
        var customer = await _handler.RegisterCustomerAsync(customerRequest);

        return Created(customer.Id.ToString(), customer);
    }


    /// <summary>
    /// Retrieves all customers.
    /// </summary>
    /// <remarks>
    /// This endpoint is used to retrieve a list of all customers.
    /// It returns a collection of customer information for all existing customers.
    /// </remarks>
    /// <returns>An action result containing a collection of customer information.</returns>
    /// <response code="200">Indicates a successful retrieval of customers and returns the collection of customer information.</response>
    [HttpGet("Customer")]
    public async Task<ActionResult<IEnumerable<CustomerInfo>>> GetAllCustomers()
    {
        var customers = await _handler.GetCustomersAsync();

        return customers.ToList();
    }

    /// <summary>
    /// Adds products to a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows adding products to a specific customer identified by the provided customer ID.
    /// The productsToAdd parameter should contain a collection of product IDs to be added to the customer's list of products.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="productsToAdd">The collection of product IDs to add.</param>
    /// <returns>An action result indicating the success of the operation.</returns>
    /// <response code="200">Indicates that the products have been successfully added to the customer.</response>
    /// <response code="400">If the customerId is empty or if productsToAdd is null.</response>
    [HttpPost("Customer/{customerId}/AddProducts")]
    public async Task<IActionResult> AddProducts([FromRoute] Guid customerId, [FromBody] IEnumerable<Guid> productsToAdd)
    {
        await _handler.AddProductsAsync(customerId, productsToAdd);

        return Ok();
    }

    /// <summary>
    /// Removes products from a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows removing products from a specific customer identified by the provided customer ID.
    /// The productsToRemove parameter should contain a collection of product IDs to be removed from the customer's list of products.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="productsToRemove">The collection of product IDs to remove.</param>
    /// <returns>An action result indicating the success of the operation.</returns>
    /// <response code="200">Indicates that the products have been successfully removed from the customer.</response>
    /// <response code="400">If the customerId is empty or if productsToRemove is null.</response>
    [HttpPost("Customer/{customerId}/RemoveProducts")]
    public async Task<IActionResult> RemoveProducts([FromRoute] Guid customerId, [FromBody] IEnumerable<Guid> productsToRemove)
    {
        await _handler.RemoveProductsAsync(customerId, productsToRemove);

        return Ok();
    }
}