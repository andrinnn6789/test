using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class WarehouseMap: ClassMapping<Warehouse>
{
    public WarehouseMap()
    {
        Schema("VinX");
        Table("Lager");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Lag_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("Lag_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
    }
}
