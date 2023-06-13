using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class PublishReleasesLogic
{
    private readonly IArtifactsScanner _artifactsScanner;
    private readonly ISettingsScanner _settingsScanner;
    private readonly IReleaseManager _releaseManager;
    private readonly IMessageLogger _messageLogger;

    public PublishReleasesLogic(IArtifactsScanner artifactsScanner, ISettingsScanner settingsScanner, IReleaseManager releaseManager, IMessageLogger messageLogger)
    {
        _artifactsScanner = artifactsScanner;
        _settingsScanner = settingsScanner;
        _releaseManager = releaseManager;
        _messageLogger = messageLogger;
    }

    public async Task PublishReleasesAsync(string artifactsPath, string settingsPath, Dictionary<string, string> configReleasePaths, SyncResult syncResult,
        IJobHeartbeatObserver jobHeartbeatObserver)
    {
        var releasesOnCc = await _releaseManager.GetReleasesAsync();
        var releasesToRemoveOnCc = releasesOnCc.Where(r => !string.IsNullOrEmpty(r.ArtifactPath) && r.ReleaseDate.HasValue)
            .ToDictionary(r => GetArtifactsKey(r.ArtifactPath, r.ReleaseVersion), r => r);

        var productsOnCc = await _releaseManager.GetProductsAsync();
        var productLookup = productsOnCc.Where(p => p.ProductType == ProductType.IagService).ToDictionary(p => p.ProductName, p => p.Id);
        var artifacts = _artifactsScanner.Scan(artifactsPath).ToList();

        // publish products
        foreach (var artifact in artifacts.Where(a => a.ProductType == ProductType.IagService || a.ProductType == ProductType.Updater))
        {
            var relKey = GetArtifactsKey(artifact.ArtifactPath, artifact.Version);
            ReleaseInfo releaseInfo;
            if (releasesToRemoveOnCc.ContainsKey(relKey))
            {
                releaseInfo = releasesToRemoveOnCc[relKey];
                releasesToRemoveOnCc.Remove(relKey);
            }
            else
            {
                configReleasePaths.TryGetValue(artifact.ProductName, out var releasePath);
                releaseInfo = await PublishRelease(syncResult, jobHeartbeatObserver, artifact, null, releasePath, artifact.Version);
            }

            if (releaseInfo != null && !productLookup.ContainsKey(artifact.ProductName))
            {
                productLookup[artifact.ProductName] = releaseInfo.ProductId;
            }
        }

        // public customer extensions
        foreach (var artifact in artifacts.Where(a => a.ProductType == ProductType.CustomerExtension))
        {
            var relKey = GetArtifactsKey(artifact.ArtifactPath, artifact.Version);
            if (releasesToRemoveOnCc.ContainsKey(relKey))
            {
                releasesToRemoveOnCc.Remove(relKey);
            }
            else
            {
                Guid? dependingProductId = null;
                string productReleasePath = null;
                if (artifact.DependingProductName != null && productLookup.ContainsKey(artifact.DependingProductName))
                {
                    dependingProductId = productLookup[artifact.DependingProductName];
                    configReleasePaths.TryGetValue(artifact.DependingProductName, out productReleasePath);
                }
                configReleasePaths.TryGetValue(artifact.ProductName, out var costumerExtensionReleasePath);
                var releasePath = Path.Combine(productReleasePath ?? string.Empty, costumerExtensionReleasePath ?? string.Empty);
                await PublishRelease(syncResult, jobHeartbeatObserver, artifact, dependingProductId, releasePath, artifact.Version);
            }
        }

        // publish configurations
        foreach (var configuration in _settingsScanner.Scan(settingsPath))
        {
            var relKey = GetArtifactsKey(configuration.ArtifactPath, configuration.Version);
            if (releasesToRemoveOnCc.ContainsKey(relKey))
            {
                releasesToRemoveOnCc.Remove(relKey);
            }
            else
            {
                if (!productLookup.ContainsKey(configuration.DependingProductName))
                {
                    _messageLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.NoProductForConfigurationWarning, configuration.ProductName, configuration.ProductName);
                    continue;
                }

                var dependingProductId = productLookup[configuration.DependingProductName];
                configReleasePaths.TryGetValue(SettingsScanner.SettingsDirectoryName, out var settingsReleasePath);
                await PublishRelease(syncResult, jobHeartbeatObserver, configuration, dependingProductId, settingsReleasePath, string.Empty);
            }
        }

        // remove old releases on CC
        foreach (var oldReleaseId in releasesToRemoveOnCc.Values)
        {
            jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
            try
            {
                await _releaseManager.RemoveReleaseAsync(oldReleaseId);
            }
            catch (Exception ex)
            {
                _messageLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.CleanupOldReleaseWarning);
                _messageLogger.AddMessage(ex);
            }
        }
    }

    private static string GetArtifactsKey(string artifactInfo, string version)
    {
        return artifactInfo + "_" + version;
    }

    private async Task<ReleaseInfo> PublishRelease(SyncResult syncResult, IJobHeartbeatObserver jobHeartbeatObserver,
        ArtifactInfo artifact, Guid? dependingProductId, string releasePath, string releaseVersion)
    {
        try
        {
            jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
            var product = await _releaseManager.CreateProductAsync(
                artifact.ProductName, artifact.ProductType, dependingProductId);

            jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
            var releaseInfo = await _releaseManager.CreateReleaseAsync(product, artifact.ArtifactPath, releasePath,
                releaseVersion, jobHeartbeatObserver);

            _messageLogger.AddMessage(MessageTypeEnum.Information, ResourceIds.ReleaseSuccessfullyPublishedInfo,
                artifact.ArtifactPath, artifact.ProductName);
            syncResult.SuccessCount++;

            return releaseInfo;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (LocalizableException lex) when (lex.Message == ResourceIds.ReleaseAlreadyApprovedError)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Information, ResourceIds.ReleaseAlreadyPublishedInfo, artifact.ArtifactPath, artifact.ProductName);
        }
        catch (Exception ex)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.PublishReleaseError, artifact.ArtifactPath, artifact.ProductName);
            _messageLogger.AddMessage(ex);
            syncResult.ErrorCount++;
        }

        return null;
    }
}