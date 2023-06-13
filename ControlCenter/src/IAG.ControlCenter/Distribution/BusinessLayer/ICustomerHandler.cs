using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public interface ICustomerHandler
{
    Task<CustomerInfo> GetCustomerAsync(Guid customerId);
    Task<IEnumerable<ProductInfo>> GetProductsAsync(Guid customerId);
    Task<IEnumerable<ReleaseInfo>> GetReleasesAsync(Guid customerId, Guid productId);
    Task<IEnumerable<FileMetaInfo>> GetReleaseFilesAsync(Guid customerId, Guid releaseId);
    Task<FileWithDataInfo> GetFileAsync(Guid customerId, Guid fileId);
    Task<InstallationInfo> RegisterInstallationAsync(Guid customerId, InstallationRegistration installation);
    Task<IEnumerable<LinkInfo>> GetLinksAsync(Guid customerId);
}