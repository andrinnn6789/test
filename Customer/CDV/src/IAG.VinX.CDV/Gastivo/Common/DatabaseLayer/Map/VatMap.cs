using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class VatMap : ClassMapping<Vat>
{
	public VatMap()
	{
		Schema("VinX");
		Table("MWST");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("MWST_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.Description, map =>
		{
			map.Column("MWST_Bezeichnung");
			map.Type(NHibernateUtil.AnsiString);
			map.NotNullable(true);
		});
		Property(x => x.Percentage, map =>
		{
			map.Column("MWST_Prozent");
			map.NotNullable(true);
		});
	}
}