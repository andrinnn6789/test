using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class OnlineOrderLineMap : ClassMapping<OnlineOrderLine>
{
	public OnlineOrderLineMap()
	{
		Schema("VinX");
		Table("OnlineBestellPosition");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("BestPos_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.Position, map => map.Column("BestPos_Position"));
		Property(x => x.Description, map =>
		{
			map.Column("BestPos_Bezeichnung");
			map.Type(NHibernateUtil.AnsiString);
			map.Length(64000);
		});
		Property(x => x.OrderedQuantity, map => map.Column("BestPos_Anzahl"));
		Property(x => x.UnitPrice, map => map.Column("BestPos_Preis"));
		Property(x => x.Total, map => map.Column("BestPos_Total"));
		Property(x => x.Vat, map => map.Column("BestPos_MWSTProzentInPreis"));
		ManyToOne(x => x.Order, map => 
		{
			map.Column("BestPos_BestID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.Article, map => 
		{
			map.Column("BestPos_ArtikelID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
	}
}