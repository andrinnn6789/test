using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class CountryMap : ClassMapping<Country>
{
    public CountryMap()
    {
        Schema("VinX");
        Table("Land");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Land_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.IsoNumber, map =>  map.Column("Land_IsoNummer"));
    }
}