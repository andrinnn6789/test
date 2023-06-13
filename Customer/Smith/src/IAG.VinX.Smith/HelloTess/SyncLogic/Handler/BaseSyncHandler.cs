using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Smith.HelloTess.DataMapper;
using IAG.VinX.Smith.HelloTess.HelloTessRest;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;
using IAG.VinX.Smith.HelloTess.VinX;
using IAG.VinX.Smith.Resource;

namespace IAG.VinX.Smith.HelloTess.SyncLogic.Handler;

public abstract class BaseSyncHandler<TSource, TTarget> where TSource : IKeyable where TTarget : ISyncableSource
{
    private readonly IJobHeartbeatObserver _jobHeartbeatObserver;
    private Dictionary<string, TTarget> _targetItemsByKey;

    protected BaseSyncHandler(IJobHeartbeatObserver jobHeartbeatObserver, IMessageLogger msgLogger, HelloTessArticleCommonSyncResult commonSyncResult)
    {
        _jobHeartbeatObserver = jobHeartbeatObserver;
        MessageLogger = msgLogger;
        CommonSyncResult = commonSyncResult;
    }
        
    protected IMessageLogger MessageLogger { get; }

    protected HelloTessArticleCommonSyncResult CommonSyncResult { get; }
        
    protected abstract string AspectNameSingularResourceId { get; }

    protected abstract string AspectNamePluralResourceId { get; }

    protected abstract int UpdateCount { get; set; }

    protected abstract int InsertCount { get; set; }

    protected abstract int SetInactiveCount { get; set; }

    protected abstract int ErrorCount { get;  set; }

    protected async Task DoSync(IVinXClient<TSource> sourceClient, IRestClient<TTarget> targetClient, IDataMapper<TSource, TTarget> dataMapper)
    {
        UpdateCount = 0;
        InsertCount = 0;
        SetInactiveCount = 0;
        ErrorCount = 0;

        IEnumerable<TSource> sourceItems;
        try
        {
            sourceItems = sourceClient.Get();
        }
        catch (Exception ex)
        {
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.SyncErrorGetSource, new LocalizableParameter(AspectNamePluralResourceId), LocalizableException.GetExceptionMessage(ex));
            MessageLogger.AddMessage(ex);
            ErrorCount++;
            return;
        }
        _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
            
        IEnumerable<TTarget> targetItems;
        try
        {
            targetItems = await targetClient.Get();
        }
        catch (Exception ex)
        {
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.SyncErrorGetTarget, new LocalizableParameter(AspectNamePluralResourceId), LocalizableException.GetExceptionMessage(ex));
            MessageLogger.AddMessage(ex);
            ErrorCount++;
            return;
        }
        _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();

        var targetItemsList = targetItems.ToList();
        var targetItemsById = targetItemsList.ToDictionary(x => x.Id);
        _targetItemsByKey = targetItemsList.ToDictionary(x => x.Key);

        var itemsToUpdate = new List<TTarget>();
        var itemsToCreate = new List<TTarget>();

        foreach (TSource sourceItem in sourceItems)
        {
            if (_targetItemsByKey.TryGetValue(sourceItem.Key, out var targetItem))
            {
                if (dataMapper.CheckUpdate(sourceItem, targetItem))
                {
                    itemsToUpdate.Add(targetItem);
                }

                targetItemsById.Remove(targetItem.Id);
            }
            else
            {
                var newItem = dataMapper.CreateTarget(sourceItem);
                itemsToCreate.Add(newItem);
            }
        }
        _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();

        foreach (var itemToSetInactive in targetItemsById)
        {
            if (!dataMapper.CheckDelete(itemToSetInactive.Value)) continue;

            try
            {
                await targetClient.Post(itemToSetInactive.Value);
                SetInactiveCount++;
            }
            catch (Exception ex)
            {
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.SyncErrorDeleteFormatMessage,
                    new LocalizableParameter(AspectNameSingularResourceId), itemToSetInactive.Key, LocalizableException.GetExceptionMessage(ex));
                MessageLogger.AddMessage(ex);
                ErrorCount++;
            }
            _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }

        foreach (var itemToUpdate in itemsToUpdate)
        {
            try
            {
                await targetClient.Post(itemToUpdate);
                UpdateCount++;
            }
            catch (Exception ex)
            {
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.SyncErrorUpdateFormatMessage, 
                    new LocalizableParameter(AspectNameSingularResourceId), itemToUpdate.Key, LocalizableException.GetExceptionMessage(ex));
                MessageLogger.AddMessage(ex);
                ErrorCount++;
            }
            _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }

        foreach (var itemToCreate in itemsToCreate)
        {
            try
            {
                await targetClient.Post(itemToCreate);
                InsertCount++;
            }
            catch (Exception ex)
            {
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.SyncErrorInsertFormatMessage, 
                    new LocalizableParameter(AspectNameSingularResourceId), itemToCreate.Key, LocalizableException.GetExceptionMessage(ex));
                MessageLogger.AddMessage(ex);
                ErrorCount++;
            }
            _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }
    }

    public JobResultEnum CheckSyncJobResult()
    {
        var successCount = UpdateCount + InsertCount + SetInactiveCount;

        if (ErrorCount > 0)
        {
            return successCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Failed;
        }

        return successCount == 0 ? JobResultEnum.NoResult : JobResultEnum.Success;
    }
}