using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class AddressMap : ClassMapping<Address>
{
    public AddressMap()
    {
        Schema("VinX");
        Table("Adresse");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Adr_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.AddressNumber, map => map.Column("Adr_Adressnummer"));
        Property(x => x.SearchTerm, map =>
        {
            map.Column("Adr_Suchbegriff");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.Name, map =>
        {
            map.Column("Adr_Name");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.FirstName, map =>
        {
            map.Column("Adr_Vorname");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.Street, map =>
        {
            map.Column("Adr_Strasse");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.ZipCode, map =>
        {
            map.Column("Adr_PLZ");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.City, map =>
        {
            map.Column("Adr_Ort");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.Company, map =>
        {
            map.Column("Adr_Firma");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.IsActive, map =>
        {
            map.Formula("ABS(Adr_Aktiv)");
            map.Type(NHibernateUtil.Boolean);
        });
        Property(x => x.TransmitToGastivo, map =>
        {
            map.Formula("ABS(Adr_UebermittlungGastivo)");
            map.Type(NHibernateUtil.Boolean);
        });
        Property(x => x.PaymentConditionId, map => map.Column("Adr_ZahlkondID"));
        Property(x => x.DeliveryConditionId, map => map.Column("Adr_LiefbedID"));
        ManyToOne(x => x.PriceGroup, map => 
        {
            map.Column("Adr_KGrpPreisID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        Bag(x => x.SpecialPrices, colmap =>
        {
            colmap.Key(x =>
            {
                x.Column("VKSpez_KundID");
                x.ForeignKey("Adr_ID");
            });
            colmap.Inverse(true);
        }, map => { map.OneToMany(); });
        ManyToOne(x => x.ConditionAddress, map =>
        {
            map.Column("Adr_KonditionenAdresseID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        }); 
        ManyToOne(x => x.BillingAddress, map =>
        {
            map.Column("Adr_RechAdrID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        Property(x => x.PriceCondition, map => map.Column("Adr_PreiseKonditionen"));
    }
}