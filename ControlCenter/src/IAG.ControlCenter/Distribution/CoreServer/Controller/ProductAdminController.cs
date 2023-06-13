using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace IAG.ControlCenter.Distribution.CoreServer.Controller;

[Route(ControlCenterEndpoints.Distribution + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
public class ProductAdminController : ControllerBase
{
    private readonly IProductAdminHandler _handler;

    public ProductAdminController(IProductAdminHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Retrieves the list of products.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the list of products.
    /// </remarks>
    /// <returns>An action result containing the list of products.</returns>
    /// <response code="200">Indicates that the list of products was successfully retrieved.</response>
    [HttpGet("Product")]
    public async Task<ActionResult<List<ProductInfo>>> GetProductsAsync()
    {
        var products = await _handler.GetProductsAsync();

        return Ok(products);
    }

    /// <summary>
    /// Retrieves the list of releases.
    /// </summary>
    /// <remarks>
    /// This endpoint allows retrieving the list of releases.
    /// </remarks>
    /// <returns>An action result containing the list of releases.</returns>
    /// <response code="200">Indicates that the list of releases was successfully retrieved.</response>
    [HttpGet("Release")]
    public async Task<ActionResult<List<ProductInfo>>> GetReleasesAsync()
    {
        var releases = await _handler.GetReleasesAsync();

        return Ok(releases);
    }

    /// <summary>
    /// Registers a new product.
    /// </summary>
    /// <remarks>
    /// This endpoint allows registering a new product by providing the necessary product information in the request body.
    /// The product information should be provided as a ProductRegistration object.
    /// </remarks>
    /// <param name="productRequest">The product information to register.</param>
    /// <returns>An action result containing the registered product information.</returns>
    /// <response code="201">Indicates that the product was successfully registered.</response>
    [HttpPost("Product")]
    public async Task<ActionResult<ProductInfo>> RegisterProduct([FromBody] ProductRegistration productRequest)
    {
        var product = await _handler.RegisterProductAsync(productRequest);

        return Created(product.Id.ToString(), product);
    }

    /// <summary>
    /// Registers a new release for a specific product.
    /// </summary>
    /// <remarks>
    /// This endpoint allows registering a new release for a specific product by providing the product ID in the route parameter and the release information in the request body.
    /// The release information should be provided as a ReleaseRegistration object.
    /// </remarks>
    /// <param name="productId">The ID of the product for which to register the release.</param>
    /// <param name="releaseRequest">The release information to register.</param>
    /// <returns>An action result containing the registered release information.</returns>
    /// <response code="201">Indicates that the release was successfully registered.</response>
    [HttpPost("Product/{productId}/Release")]
    public async Task<ActionResult<ReleaseInfo>> RegisterRelease([FromRoute] Guid productId, [FromBody] ReleaseRegistration releaseRequest)
    {
        var release = await _handler.RegisterReleaseAsync(productId, releaseRequest);

        return Created(release.Id.ToString(), release);
    }

    /// <summary>
    /// Adds files to a specific release.
    /// </summary>
    /// <remarks>
    /// This endpoint allows adding multiple files to a specific release by providing the release ID in the route parameter and the file information in the request body.
    /// The file information should be provided as a list of FileRegistration objects.
    /// </remarks>
    /// <param name="releaseId">The ID of the release to which the files should be added.</param>
    /// <param name="fileRegistrations">The list of file information to add.</param>
    /// <returns>An action result containing the added files.</returns>
    [HttpPost("Product/{productId}/Release/{releaseId}/AddFiles")]
    public async Task<ActionResult<List<FileMetaInfo>>> AddFiles([FromRoute] Guid releaseId, [FromBody] List<FileRegistration> fileRegistrations)
    {
        var files = await _handler.AddFilesToReleaseAsync(releaseId, fileRegistrations);

        return files.ToList();
    }

    /// <summary>
    /// Sets the content of a file.
    /// </summary>
    /// <remarks>
    /// This endpoint allows setting the content of a file by providing the file ID in the route parameter and the file content in the request body.
    /// The file content should be provided as a form file.
    /// </remarks>
    /// <param name="fileId">The ID of the file for which the content should be set.</param>
    /// <param name="file">The form file containing the file content.</param>
    /// <returns>An action result containing the updated file information.</returns>
    [HttpPost("File/{fileId}")]
    public async Task<ActionResult<FileMetaInfo>> SetFileContent([FromRoute] Guid fileId, [FromForm] IFormFile file)
    {
        await using MemoryStream memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var fileInfo = await _handler.SetFileContentAsync(fileId, memoryStream.ToArray());

        return fileInfo;
    }

    /// <summary>
    /// Approves a release.
    /// </summary>
    /// <remarks>
    /// This endpoint allows approving a release by providing the release ID in the route parameter.
    /// </remarks>
    /// <param name="releaseId">The ID of the release to be approved.</param>
    /// <returns>An action result containing the approved release information.</returns>
    [HttpPost("Product/{productId}/Release/{releaseId}/Approve")]
    public async Task<ActionResult<ReleaseInfo>> ApproveRelease([FromRoute] Guid releaseId)
    {
        var release = await _handler.ApproveReleaseAsync(releaseId);

        return release;
    }

    /// <summary>
    /// Removes a release.
    /// </summary>
    /// <remarks>
    /// This endpoint allows removing a release by providing the release ID in the route parameter.
    /// </remarks>
    /// <param name="releaseId">The ID of the release to be removed.</param>
    /// <returns>An IActionResult indicating the result of the removal operation.</returns>
    [HttpPost("Product/{productId}/Release/{releaseId}/Remove")]
    public async Task<IActionResult> RemoveRelease([FromRoute] Guid releaseId)
    {
        await _handler.RemoveReleaseAsync(releaseId);

        return Ok();
    }

}