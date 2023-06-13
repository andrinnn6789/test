using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class StockMovementMap : ClassMapping<StockMovement>
{
	public StockMovementMap()
	{
		Schema("VinX");
		Table("Bewegung");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("Bew_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.Date, map =>
		{
			map.Column("Bew_Datum");
			map.Type(NHibernateUtil.Date);
			map.NotNullable(true);
		});
		Property(x => x.MovementType, map =>
		{
			map.Column("Bew_Vorgang");
			map.NotNullable(true);
		});
		Property(x => x.BulkPackageQuantity, map => map.Column("Bew_MengeGG"));
		Property(x => x.Quantity, map => map.Column("Bew_Menge"));
		ManyToOne(x => x.Article, map =>
		{
			map.Column("Bew_ArtikelID");
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.Warehouse, map =>
		{
			map.Column("Bew_LagerID");
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.Address, map =>
		{
			map.Column("Bew_AdresseID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
	}
}