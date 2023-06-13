using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class RegionMap : ClassMapping<Region>
{
    public RegionMap()
    {
        Schema("VinX");
        Table("Region");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Reg_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Name, map =>
        {
            map.Column("Reg_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.NameFrench, map =>
                {
                    map.Column("Reg_BezeichnungFR");
                    map.Type(NHibernateUtil.AnsiString);
                });
        Property(x => x.NameItalian, map =>
        {
            map.Column("Reg_BezeichnungIT");
            map.Type(NHibernateUtil.AnsiString);
        });
    }
}