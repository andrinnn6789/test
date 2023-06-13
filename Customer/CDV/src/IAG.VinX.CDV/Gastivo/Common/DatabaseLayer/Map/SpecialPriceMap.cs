using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class SpecialPriceMap : ClassMapping<SpecialPrice>
{
	public SpecialPriceMap()
	{
		Schema("VinX");
		Table("VKspezPreis");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("VKSpez_ID");
			map.Generator(Generators.Identity);
		});
        Property(x => x.Price, map =>
		{
			map.Column("VKSpez_Preis");
			map.NotNullable(true);
		});
		Property(x => x.ValidFrom, map =>
		{
			map.Column("VKSpez_DatumVon");
			map.Type(NHibernateUtil.Date);
		});
		Property(x => x.ValidTo, map =>
		{
			map.Column("VKSpez_DatumBis");
			map.Type(NHibernateUtil.Date);
		});
		ManyToOne(x => x.Article, map =>
		{
			map.Column("VKSpez_ArtID");
			map.Cascade(Cascade.None);
		});
    }
}