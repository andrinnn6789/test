using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public class ProductAdminClient : RestClient, IProductAdminClient
{
    private const string ProductAdminEndpoint = "ProductAdmin";

    public ProductAdminClient(IHttpConfig config, IRequestResponseLogger logger = null) : base(config, logger)
    {
    }

    public async Task<List<ProductInfo>> GetProductsAsync()
    {
        try
        {
            return await GetAsync<List<ProductInfo>>(ProductAdminEndpoint + "/Product");
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.GetProductsError, ex);
        }
    }

    public async Task<List<ReleaseInfo>> GetReleasesAsync()
    {
        try
        {
            return await GetAsync<List<ReleaseInfo>>(ProductAdminEndpoint + "/Release");
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.GetReleasesError, ex);
        }
    }

    public async Task<ProductInfo> RegisterProductAsync(string productName, ProductType productType, Guid? dependsOnProductId = null)
    {
        try
        {
            var requestBody = new ProductRegistration
            {
                ProductName = productName,
                Description = $"Product created by ProcessEngine Job at {DateTime.Now}",
                Type = productType,
                DependsOnProductId = dependsOnProductId
            };
            var request = new JsonRestRequest(HttpMethod.Post, ProductAdminEndpoint + "/Product");
            request.SetJsonBody(requestBody);
            var productInfo = await PostAsync<ProductInfo>(request);

            return productInfo;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RegisterProductError, ex);
        }
    }

    public async Task<ReleaseInfo> RegisterReleaseAsync(Guid productId, string releaseVersion, string platform, string description, string releasePath, string artifactPath)
    {
        try
        {
            var requestBody = new ReleaseRegistration
            {
                ReleaseVersion = releaseVersion,
                Platform = platform,
                Description = description,
                ReleasePath = releasePath,
                ArtifactPath = artifactPath
            };
            var request = new JsonRestRequest(HttpMethod.Post, ProductAdminEndpoint + $"/Product/{productId}/Release");
            request.SetJsonBody(requestBody);

            return await PostAsync<ReleaseInfo>(request);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RegisterReleaseError, ex);
        }
    }

    public async Task<List<FileMetaInfo>> AddFilesToReleaseAsync(Guid productId, Guid releaseId, List<FileRegistration> files)
    {
        try
        {
            var request = new JsonRestRequest(HttpMethod.Post, ProductAdminEndpoint + $"/Product/{productId}/Release/{releaseId}/AddFiles");
            request.SetJsonBody(files);

            return await PostAsync<List<FileMetaInfo>>(request);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.AddFilesToReleaseError, ex, productId, releaseId);
        }
    }

    public async Task SetFileContentAsync(Guid fileId, byte[] content)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ProductAdminEndpoint + $"/File/{fileId}");
            var fileContent = new MultipartFormDataContent
            {
                { new ByteArrayContent(content), "file", "DoesNotMatter.foo" }
            };
            request.Content = fileContent;
            PrepareRequest(request);
            var response = await SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.SetFileContentError, fileId, ex, fileId);
        }
    }

    public async Task ApproveReleaseAsync(Guid productId, Guid releaseId)
    {
        try
        {
            var request = new JsonRestRequest(HttpMethod.Post, ProductAdminEndpoint + $"/Product/{productId}/Release/{releaseId}/Approve");
            await PostAsync<ReleaseInfo>(request);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ApproveReleaseError, ex, productId, releaseId);
        }
    }

    public async Task RemoveReleaseAsync(Guid productId, Guid releaseId)
    {
        try
        {
            var request = new JsonRestRequest(HttpMethod.Post, ProductAdminEndpoint + $"/Product/{productId}/Release/{releaseId}/Remove");
            PrepareRequest(request);
            var response = await SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RemoveReleaseError, ex, productId, releaseId);
        }
    }
}