using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Zweifel.S1M.Dto.S1M;

namespace IAG.VinX.Zweifel.S1M.BusinessLogic;

public class S1MDeliveryComposer : IS1MDeliveryComposer
{
    private ISybaseConnection _sybaseConnection;

    public void SetConfig(ISybaseConnection sybaseConnection)
    {
        _sybaseConnection = sybaseConnection;
    }

    public IEnumerable<S1MExtDelivery> ComposeDeliveries(IEnumerable<S1MDelivery> deliveries)
    {
        var extDeliveries = new List<S1MExtDelivery>();
        foreach (var delivery in deliveries)
        {
            try
            {
                var halts = _sybaseConnection.GetQueryable<S1MHalt>().Where(h => h.DeliveryId == delivery.DeliveryId)
                    .ToList();
                foreach (var halt in halts)
                {
                    // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault - FirstOrDefault is not implemented in our LINQ Driver
                    halt.S1MDocument = _sybaseConnection.GetQueryable<S1MDocument>()
                        .Where(d => d.Id == halt.DocumentId).FirstOrDefault();
                    halt.S1MDocument!.ArticlePositions = _sybaseConnection.GetQueryable<S1MArticlePosition>()
                        .Where(p => p.DocumentId == halt.DocumentId).ToList();
                    halt.S1MDocument.BulkPackagePositions = _sybaseConnection.GetQueryable<S1MBulkPackagePosition>()
                        .Where(b => b.DocumentId == halt.DocumentId).ToList();
                    // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault - FirstOrDefault is not implemented in our LINQ Driver
                    halt.S1MAddress = _sybaseConnection.GetQueryable<S1MAddress>()
                        .Where(a => a.Id == halt.AddressId).FirstOrDefault();
                    var contactPersons = _sybaseConnection.GetQueryable<ContactPerson>()
                        .Where(c => c.Id == halt.S1MAddress.ContactPersonId && c.AddressId == halt.S1MAddress.Id ||
                                    c.DepartmentId == 13 && c.AddressId == halt.S1MAddress.Id)
                        .ToList();
                    halt.S1MAddress!.ContactPersons = contactPersons;

                    halt.Weight = halt.S1MDocument.ArticlePositions.Sum(position => position.Weight);
                }

                var extDelivery = new S1MExtDelivery
                {
                    DeliveryId = delivery.DeliveryId,
                    TourNumber = delivery.TourNumber,
                    TourName = delivery.TourName,
                    Vehicle = _sybaseConnection.GetQueryable<Vehicle>().FirstOrDefault(v => v.Id == delivery.VehicleId),
                    DriverUref = delivery.DriverUref,
                    Status = delivery.Status,
                    DeliveryDate = delivery.DeliveryDate,
                    StartKms = delivery.StartKms,
                    EndKms = delivery.EndKms,
                    StartTime = delivery.StartTime,
                    EndTime = delivery.EndTime,
                    Halts = halts,
                    Weight = halts.Sum(halt => halt.Weight)
                };
                extDeliveries.Add(extDelivery);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error while mapping delivery with ID {delivery.DeliveryId}", e);
            }
        }

        return extDeliveries;
    }
}