using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class OnlineOrderMap : ClassMapping<OnlineOrder>
{
	public OnlineOrderMap()
	{
		Schema("VinX");
		Table("OnlineBestellung");
		Lazy(true);
		Id(x => x.Id, map =>
		{
			map.Column("Best_ID");
			map.Generator(Generators.Identity);
		});
		Property(x => x.OrderReference, map =>
		{
			map.Column("Best_BestellReferenz");
			map.Type(NHibernateUtil.AnsiString);
		});
		Property(x => x.OrderDate, map =>
		{
			map.Column("Best_Datum");
			map.Type(NHibernateUtil.Date);
			map.NotNullable(true);
		});
		Property(x => x.DeliveryDateRequested, map =>
		{
			map.Column("Best_Lieferdatum");
			map.Type(NHibernateUtil.Date);
			map.NotNullable(true);
		});
		Property(x => x.Hint, map =>
		{
			map.Column("Best_Hinweis");
			map.Type(NHibernateUtil.AnsiString);
			map.Length(64000);
		});
		Property(x => x.PaymentConditionId, map => map.Column("Best_ZahlungskonditionID"));
		Property(x => x.DeliveryConditionId, map => map.Column("Best_LieferbedingungID"));
		Property(x => x.DivisionId, map => map.Column("Best_BereichID"));
		Property(x => x.ProviderId, map => map.Column("Best_ProviderID"));
		Property(x => x.IsProcessed, map => map.Column("Best_Verarbeitet"));
		Property(x => x.IsVatIncluded, map => map.Column("Best_InklMWST"));
		Property(x => x.NumberOfLines, map => map.Column("Best_Positionen"));
		ManyToOne(x => x.OrderingAddress, map => 
		{
			map.Column("Best_AdrID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.DeliveryAddress, map => 
		{
			map.Column("Best_LieferAdresseID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
		ManyToOne(x => x.ConditionAddress, map => 
		{
			map.Column("Best_KonditionenAdresseID");
			map.NotNullable(true);
			map.Cascade(Cascade.None);
		});
		Bag(x => x.OnlineOrderLines, colmap =>
		{
			colmap.Key(x =>
			{
				x.Column("BestPos_BestID");
				x.ForeignKey("Best_ID");
			});
			colmap.Inverse(true);
			colmap.Cascade(Cascade.All);
		}, map => { map.OneToMany(); });
	}
}