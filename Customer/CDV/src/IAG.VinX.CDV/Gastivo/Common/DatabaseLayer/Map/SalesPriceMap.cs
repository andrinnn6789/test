using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class SalesPriceMap : ClassMapping<SalesPrice>
{
	public SalesPriceMap()
	{
		Schema("VinX");
		Table("VKpreis");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("VK_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.Price, map =>
		{
			map.Column("VK_Preis");
			map.NotNullable(true);
		});
		Property(x => x.IsActive, map =>
		{
			map.Formula("ABS(VK_Aktiv)");
			map.Type(NHibernateUtil.Boolean);
		});
		Property(x => x.ValidFrom, map =>
		{
			map.Column("VK_VonDatum");
			map.Type(NHibernateUtil.Date);
		});
		Property(x => x.ValidTo, map =>
		{
			map.Column("VK_BisDatum");
			map.Type(NHibernateUtil.Date);
		});
		ManyToOne(x => x.Article, map =>
		{
			map.Column("VK_ArtID");
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.PriceGroup, map =>
		{
			map.Column("VK_KGrpPreisID");
			map.Cascade(Cascade.None);
		});
	}
}