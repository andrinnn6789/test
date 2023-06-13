using System.Collections.Generic;

using IAG.Common.DataLayerSybase;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;

public class CustomerRepository : BaseSybaseRepository, ICustomerRepository
{
    public CustomerRepository(ISybaseConnection sybaseConnection) : base(sybaseConnection)
    {
    }

    public IEnumerable<IagCustomer> GetCustomers()
    {
        var sql = @"SELECT 
                            Adr_GUID AS CustomerId,
                            IsNull(Adr_Suchbegriff, Adr_Name) AS CustomerName,
                            Adr_KKatID AS CustomerCategoryId,
                            KK.KundKat_Bezeichnung AS Description,
                            CASE WHEN Adr_KKatID=20 OR Adr_ID=9999 THEN 1 ELSE 0 END AS UsesVinX,
                            CASE WHEN Adr_KKatID=127 THEN 1 ELSE 0 END AS UsesPerformX
                        FROM Adresse
                        JOIN Kundenkategorie KK ON KK.KundKat_ID=Adr_KKatID
                        WHERE Adr_KKatID IN (20, 127) OR Adr_ID=9999";

        return SybaseConnection.QueryBySql<IagCustomer>(sql);
    }
}