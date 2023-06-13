using System.Collections.Generic;

using IAG.Common.DataLayerSybase;
using IAG.VinX.Zweifel.S1M.Dto.S1M;

namespace IAG.VinX.Zweifel.S1M.BusinessLogic;

public interface IS1MDeliveryComposer
{
    void SetConfig(ISybaseConnection sybaseConnection);
    
    IEnumerable<S1MExtDelivery> ComposeDeliveries(IEnumerable<S1MDelivery> deliveries);
}