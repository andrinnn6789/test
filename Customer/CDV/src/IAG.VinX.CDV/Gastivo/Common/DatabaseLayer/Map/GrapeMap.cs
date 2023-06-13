using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class GrapeMap : ClassMapping<Grape>
{
    public GrapeMap()
    {
        Schema("VinX");
        Table("Traubensorte");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Sorte_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("Sorte_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.DescriptionFrench, map =>
                {
                    map.Column("Sorte_BezeichnungFR");
                    map.Type(NHibernateUtil.AnsiString);
                });
        Property(x => x.DescriptionItalian, map =>
        {
            map.Column("Sorte_BezeichnungIT");
            map.Type(NHibernateUtil.AnsiString);
        });
    }
}