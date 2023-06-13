using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public interface IProductAdminClient
{
    Task<List<ProductInfo>> GetProductsAsync();
    Task<List<ReleaseInfo>> GetReleasesAsync();
    Task<ProductInfo> RegisterProductAsync(string productName, ProductType productType, Guid? dependsOnProductId = null);
    Task<ReleaseInfo> RegisterReleaseAsync(Guid productId, string releaseVersion, string platform, string description, string releasePath, string artifactPath);
    Task<List<FileMetaInfo>> AddFilesToReleaseAsync(Guid productId, Guid releaseId, List<FileRegistration> files);
    Task SetFileContentAsync(Guid fileId, byte[] content);
    Task ApproveReleaseAsync(Guid productId, Guid releaseId);
    Task RemoveReleaseAsync(Guid productId, Guid releaseId);
}