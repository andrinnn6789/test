using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface IReleaseManager
{
    Task<IEnumerable<ProductInfo>> GetProductsAsync(Guid customerId);
    Task<IEnumerable<ReleaseInfo>> GetReleasesAsync(Guid customerId, Guid productId);
    Task<IEnumerable<FileMetaInfo>> GetReleaseFilesAsync(Guid customerId, Guid productId, Guid releaseId);
    Task<Stream> GetFileContentStreamAsync(Guid customerId, Guid fileId);
}