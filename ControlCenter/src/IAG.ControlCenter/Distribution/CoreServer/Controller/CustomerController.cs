using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace IAG.ControlCenter.Distribution.CoreServer.Controller;

[Route(ControlCenterEndpoints.Distribution + "[controller]")]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
[AllowAnonymous]
public class CustomerController : ControllerBase
{
    private readonly ICustomerHandler _handler;

    public CustomerController(ICustomerHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Retrieves customer information.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving information about a specific customer identified by the provided customer ID.
    /// The customer ID should be included in the route parameter.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>An action result containing the customer information.</returns>
    /// <response code="200">Indicates that the customer information has been successfully retrieved.</response>
    /// <response code="401">If access to the customer with the specified ID is not allowed.</response>
    /// <response code="404">If the customer with the specified ID does not exist.</response>
    [HttpGet("{customerId}")]
    public async Task<ActionResult<CustomerInfo>> GetCustomer([FromRoute] Guid customerId)
    {
        return await _handler.GetCustomerAsync(customerId);
    }

    /// <summary>
    /// Retrieves the products associated with a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the products associated with a specific customer identified by the provided customer ID.
    /// The customer ID should be included in the route parameter.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>An action result containing the list of products associated with the customer.</returns>
    /// <response code="200">Indicates that the products associated with the customer have been successfully retrieved.</response>
    /// <response code="401">If access to the customer with the specified ID is not allowed.</response>
    /// <response code="404">If the customer with the specified ID does not exist.</response>
    [HttpGet("{customerId}/Product")]
    public async Task<ActionResult<IEnumerable<ProductInfo>>> GetProducts([FromRoute] Guid customerId)
    {
        var products = await _handler.GetProductsAsync(customerId);
        return products.ToList();
    }

    /// <summary>
    /// Retrieves the releases associated with a specific product for a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the releases associated with a specific product for a customer.
    /// The customer ID and product ID should be included in the route parameters.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>An action result containing the list of releases associated with the product for the customer.</returns>
    /// <response code="200">Indicates that the releases associated with the product for the customer have been successfully retrieved.</response>
    /// <response code="401">If access to the product or customer is not allowed.</response>
    /// <response code="404">If the product with the specified ID does not exist.</response>
    [HttpGet("{customerId}/Product/{productId}/Release")]
    public async Task<ActionResult<IEnumerable<ReleaseInfo>>> GetReleases([FromRoute] Guid customerId, [FromRoute] Guid productId)
    {
        var releases = await _handler.GetReleasesAsync(customerId, productId);
        return releases.ToList();
    }

    /// <summary>
    /// Retrieves the files associated with a specific release for a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the files associated with a specific release for a customer.
    /// The customer ID, product ID, and release ID should be included in the route parameters.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="releaseId">The ID of the release.</param>
    /// <returns>An action result containing the list of files associated with the release for the customer.</returns>
    /// <response code="200">Indicates that the files associated with the release for the customer have been successfully retrieved.</response>
    /// <response code="401">If access to the customer or release is not allowed.</response>
    /// <response code="404">If the release with the specified ID does not exist.</response>
    [HttpGet("{customerId}/Product/{productId}/Release/{releaseId}/File")]
    public async Task<ActionResult<IEnumerable<FileMetaInfo>>> GetReleaseFiles([FromRoute] Guid customerId, [FromRoute] Guid productId, [FromRoute] Guid releaseId)
    {
        var files = await _handler.GetReleaseFilesAsync(customerId, releaseId);
        return files.ToList();
    }

    /// <summary>
    /// Retrieves the content of a specific file for a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the content of a specific file for a customer.
    /// The customer ID and file ID should be included in the route parameters.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>An action result containing the content of the file for the customer.</returns>
    /// <response code="200">Indicates that the content of the file for the customer has been successfully retrieved.</response>
    /// <response code="401">If access to the customer, release, or file is not allowed.</response>
    /// <response code="404">If the file with the specified ID does not exist.</response>
    [HttpGet("{customerId}/File/{fileId}")]
    public async Task<IActionResult> GetFileContent([FromRoute] Guid customerId, [FromRoute] Guid fileId)
    {
        var file = await _handler.GetFileAsync(customerId, fileId);
        return File(file.Content, "application/octet-stream", file.Name);
    }

    /// <summary>
    /// Registers a new installation for a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows registering a new installation for a customer.
    /// The customer ID should be included in the route parameters, and the installation details should be provided in the request body.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="installation">The installation details.</param>
    /// <returns>An action result containing the information of the registered installation.</returns>
    /// <response code="201">Indicates that the installation has been successfully registered.</response>
    /// <response code="400">If the instance name is missing or empty in the installation details.</response>
    /// <response code="401">If access to the installation is not allowed.</response>
    [HttpPost("{customerId}/Installation")]
    public async Task<ActionResult<InstallationInfo>> RegisterInstallation([FromRoute] Guid customerId, [FromBody] InstallationRegistration installation)
    {
        var result = await _handler.RegisterInstallationAsync(customerId, installation);
        return Created(result.Id.ToString(), result);
    }

    /// <summary>
    /// Retrieves the links for a customer.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the links associated with a customer.
    /// The customer ID should be included in the route parameters.
    /// </remarks>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>An action result containing the links associated with the customer.</returns>
    /// <response code="200">Indicates that the links have been successfully retrieved.</response>
    /// <response code="401">If access to the customer is not allowed.</response>
    [HttpGet("{customerId}/Link")]
    public async Task<ActionResult<IEnumerable<LinkInfo>>> GetLinks([FromRoute] Guid customerId)
    {
        var links = await _handler.GetLinksAsync(customerId);
        return links.ToList();
    }
}