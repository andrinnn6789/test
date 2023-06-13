using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class ReleaseManager : IReleaseManager
{
    private const string IgnoredCustomerFileEnding = ".deps.json";

    private readonly IMessageLogger _messageLogger;
    private readonly IProductAdminClient _productAdminClient;
    private readonly ICustomerAdminClient _customerAdminClient;

    public ReleaseManager(IProductAdminClient productAdminClient, ICustomerAdminClient customerAdminClient, IMessageLogger messageLogger)
    {
        _messageLogger = messageLogger;
        _productAdminClient = productAdminClient;
        _customerAdminClient = customerAdminClient;
    }

    public Task<List<ProductInfo>> GetProductsAsync() => _productAdminClient.GetProductsAsync();

    public Task<List<ReleaseInfo>> GetReleasesAsync() => _productAdminClient.GetReleasesAsync();

    public Task<ProductInfo> CreateProductAsync(string productName, ProductType productType, Guid? dependsOnProductId = null)
    {
        return _productAdminClient.RegisterProductAsync(productName, productType, dependsOnProductId);
    }

    public async Task<ReleaseInfo> CreateReleaseAsync(ProductInfo product,
        string artifactPath, string releasePath, string releaseVersion,
        IJobHeartbeatObserver jobHeartbeatObserver)
    {
        var fileCollector = new FileCollector(artifactPath);
        var platform = fileCollector.GetPlatform();
        var description = $"Release created by ProcessEngine Job at {DateTime.Now}";
        var releaseInfo = await _productAdminClient.RegisterReleaseAsync(product.Id, releaseVersion, platform, description, releasePath, artifactPath);

        if (releaseInfo.ReleaseDate.HasValue)
        {
            throw new LocalizableException(ResourceIds.ReleaseAlreadyApprovedError, releaseVersion);
        }

        var isCustomerExtension = product.ProductType == ProductType.CustomerExtension;
        var files = fileCollector.GetFiles();
        if (isCustomerExtension)
        {
            files = files.Where(f => !f.Name.EndsWith(IgnoredCustomerFileEnding)).ToList();
        }
        _messageLogger.AddMessage(MessageTypeEnum.Information, ResourceIds.StartAddingFilesToReleaseInfo, files.Count, releaseInfo);
        Guid? customerId = null;
        var filesToProvide = await _productAdminClient.AddFilesToReleaseAsync(product.Id, releaseInfo.Id, files);
        using var assemblyInspector = new AssemblyInspector(fileCollector.BasePath);
        foreach (var file in filesToProvide)
        {
            var fileContent = await fileCollector.GetFileContentAsync(file.Name);
            await _productAdminClient.SetFileContentAsync(file.Id, fileContent);

            _messageLogger.AddMessage(MessageTypeEnum.Information, ResourceIds.SetFileContentInfo, file.Name, fileContent.Length);

            if (isCustomerExtension && !customerId.HasValue && file.Name.StartsWith("IAG.") && file.Name.EndsWith(".dll"))
            {
                var assembly = assemblyInspector.GetAssembly(fileContent);
                customerId = AssemblyInspector.GetCustomerPluginId(assembly);
            }
            jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }

        if (customerId.HasValue)
        {
            await _customerAdminClient.AddProductsAsync(customerId.Value, new[] {product.Id});
        }

        await _productAdminClient.ApproveReleaseAsync(product.Id, releaseInfo.Id);

        return releaseInfo;
    }

    public Task RemoveReleaseAsync(ReleaseInfo release) 
        => _productAdminClient.RemoveReleaseAsync(release.ProductId, release.Id);
}