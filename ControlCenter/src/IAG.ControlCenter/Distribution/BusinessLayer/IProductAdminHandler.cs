using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public interface IProductAdminHandler
{
    Task<List<ProductInfo>> GetProductsAsync();
    Task<List<ReleaseInfo>> GetReleasesAsync();
    Task<ProductInfo> RegisterProductAsync(ProductRegistration product);
    Task<ReleaseInfo> RegisterReleaseAsync(Guid productId, ReleaseRegistration release);
    Task<IEnumerable<FileMetaInfo>> AddFilesToReleaseAsync(Guid releaseId, IEnumerable<FileRegistration> files);
    Task<FileMetaInfo> SetFileContentAsync(Guid fileId, byte[] content);
    Task<ReleaseInfo> ApproveReleaseAsync(Guid releaseId);
    Task RemoveReleaseAsync(Guid releaseId);
}