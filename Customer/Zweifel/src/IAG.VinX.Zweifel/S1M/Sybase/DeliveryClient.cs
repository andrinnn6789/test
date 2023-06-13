using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;
using IAG.VinX.Zweifel.S1M.Dto.Sybase;
// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace IAG.VinX.Zweifel.S1M.Sybase;

public class DeliveryClient : IDeliveryClient
{
    private ISybaseConnection _connection;
    private List<SpecialCondition> _specialConditionsList;

    public void SetConfig(ISybaseConnection sybaseConnection)
    {
        _connection = sybaseConnection;
        _specialConditionsList = _connection.GetQueryable<SpecialCondition>().ToList();
    }

    public void MarkAsDelivered(int deliveryId, MarkDeliveredRequestModel reqModel)
    {
        UpdateDeliveryInformation(deliveryId, reqModel);

        if (reqModel.Breakages != null)
            InsertBreakagesAsCounterPositions(reqModel.Breakages);
        if (reqModel.Returns != null)
            CreateNewReturns(reqModel.Returns);
    }

    private void UpdateDeliveryInformation(int deliveryId, MarkDeliveredRequestModel reqModel)
    {
        var sybaseDelivery = new ZweifelDelivery
        {
            Id = deliveryId,
            Status = DeliveryStatus.Delivered,
            StartKms = reqModel.StartKms,
            EndKms = reqModel.EndKms,
            StartTime = reqModel.StartTime,
            EndTime = reqModel.EndTime
        };

        _connection.Update(sybaseDelivery);
    }

    private void InsertBreakagesAsCounterPositions(IEnumerable<Breakage> breakages)
    {
        foreach (var breakage in breakages)
        {
            var artPos = _connection.GetQueryable<ArticlePosition>().FirstOrDefault(ap => ap.Id == breakage.ArtPosId);
            var counterPos = new ArticlePosition
            {
                ArticleId = artPos!.ArticleId,
                DocumentId = artPos.DocumentId,
                Text = artPos.Text + " " + _specialConditionsList
                    .FirstOrDefault(sc => sc.Id == breakage.SpecialConditionId)?.Description,
                Quantity = breakage.Quantity * -1,
                QuantityBulkPackages = breakage.QuantityBulkPackages * -1,
                Price = artPos.Price,
                VatId = artPos.VatId,
                SpecialConditionId = breakage.SpecialConditionId,
                FillingId = artPos.FillingId,
                WarehouseId = artPos.WarehouseId
            };
            _connection.Insert(counterPos);
        }
    }

    private void CreateNewReturns(IEnumerable<Return> returns)
    {
        foreach (var ret in returns)
        {
            var existingBulkPackagePos = _connection.GetQueryable<BulkPackagePosition>().Where(x => x.ArticleId == ret.BulkPackageId && x.DocumentId == ret.DocumentId).FirstOrDefault();
            if (existingBulkPackagePos != null)
            {
                existingBulkPackagePos.Returned = ret.QuantityReturned;
                _connection.Update(existingBulkPackagePos);
                continue;
            }
            var article = _connection.GetQueryable<Article>().Where(a => a.Id == ret.BulkPackageId).FirstOrDefault();
            if (article == null) continue;
            var bulkPackagePos = new BulkPackagePosition
            {
                ArticleId = ret!.BulkPackageId,
                Price = article.BasePrice ?? 0,
                DocumentId = ret.DocumentId,
                Returned = ret.QuantityReturned,
                VatId = article.VatSellId
            };
            _connection.Insert(bulkPackagePos);
        }
    }
}