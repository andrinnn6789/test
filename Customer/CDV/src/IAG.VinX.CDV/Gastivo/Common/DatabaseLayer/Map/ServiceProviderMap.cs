using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class ServiceProviderMap : ClassMapping<ServiceProvider>
{
	public ServiceProviderMap()
	{
		Schema("VinX");	
		Table("Provider");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("Provider_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.Description, map =>
		{
			map.Column("Provider_Bezeichnung");
			map.Type(NHibernateUtil.AnsiString);
			map.NotNullable(true);
		});
    }
}