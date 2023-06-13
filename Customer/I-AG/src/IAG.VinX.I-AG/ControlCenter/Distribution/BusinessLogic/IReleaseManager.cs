using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ProcessEngine.Execution;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface IReleaseManager
{
    Task<List<ProductInfo>> GetProductsAsync();
    Task<List<ReleaseInfo>> GetReleasesAsync();
    Task<ProductInfo> CreateProductAsync(string productName, ProductType productType, Guid? dependsOnProductId = null);
    Task<ReleaseInfo> CreateReleaseAsync(ProductInfo product, string artifactPath, string releasePath, string releaseVersion, IJobHeartbeatObserver jobHeartbeatObserver);
    Task RemoveReleaseAsync(ReleaseInfo release);
}