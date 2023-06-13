using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class StockMap : ClassMapping<Stock>
{
    public StockMap()
    {
        Schema("VinX");
        Table("Lagerbestand");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Lagbest_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.OnStock, map => map.Column("Lagbest_Bestand"));
        Property(x => x.Reserved, map => map.Column("Lagbest_Reserviert"));
        Property(x => x.Ordered, map => map.Column("Lagbest_Bestellt"));
        Property(x => x.Provision, map => map.Column("Lagbest_Rueckstellung"));
        Property(x => x.MininumStockWebshop, map => map.Column("Lagbest_MindestbestandWebshop"));
        ManyToOne(x => x.Article, map =>
        {
            map.Column("Lagbest_ArtikelID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Warehouse, map =>
        {
            map.Column("Lagbest_LagerID");
            map.Cascade(Cascade.None);
        });
    }
}
