using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class RecommendationMap : ClassMapping<Recommendation>
{
    public RecommendationMap()
    {
        Schema("VinX");
        Table("Empfehlung");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Empf_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("Empf_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.DescriptionFrench, map =>
                {
                    map.Column("Empf_BezeichnungFR");
                    map.Type(NHibernateUtil.AnsiString);
                });
        Property(x => x.DescriptionItalian, map =>
        {
            map.Column("Empf_BezeichnungIT");
            map.Type(NHibernateUtil.AnsiString);
        });
    }
}