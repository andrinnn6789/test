using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class PriceGroupMap : ClassMapping<PriceGroup>
{
    public PriceGroupMap()
    {
        Schema("VinX");
        Table("KundengruppePreis");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("KundPreis_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("KundPreis_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.Sort, map =>
        {
            map.Column("KundPreis_Sort");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
    }
}