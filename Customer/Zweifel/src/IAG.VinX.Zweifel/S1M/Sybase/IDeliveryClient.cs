using IAG.Common.DataLayerSybase;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;

namespace IAG.VinX.Zweifel.S1M.Sybase;

public interface IDeliveryClient
{
    void SetConfig(ISybaseConnection sybaseConnection);
    void MarkAsDelivered(int deliveryId, MarkDeliveredRequestModel reqModel);
}