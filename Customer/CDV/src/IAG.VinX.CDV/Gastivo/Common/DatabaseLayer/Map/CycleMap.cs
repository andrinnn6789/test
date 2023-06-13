using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class CycleMap : ClassMapping<Cycle>
{
    public CycleMap()
    {
        Schema("VinX");
        Table("Zyklus");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Zyk_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("Zyk_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.IsActive, map =>
        {
            map.Formula("ABS(Zyk_Aktiv)");
            map.Type(NHibernateUtil.Boolean);
        });
        Property(x => x.PriceList, map => map.Column("Zyk_Preisliste"));
    }
}