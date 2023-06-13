using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class ArticleMap : ClassMapping<Article>
{
    public ArticleMap()
    {
        Schema("VinX");
        Table("Artikel");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Art_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.ArticleNumber, map => map.Column("Art_Artikelnummer"));
        Property(x => x.SearchTerm, map =>
        {
            map.Column("Art_Suchbegriff");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.Description, map =>
        {
            map.Column("Art_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
            map.Length(64000);
        });
        Property(x => x.ProductTitle, map =>
        {
            map.Column("Art_ProduktTitel");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.ProductTitleFrench, map =>
        {
            map.Column("Art_ProduktTitelFR");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.ProductTitleItalian, map =>
        {
            map.Column("Art_ProduktTitelIT");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.ArticleType, map =>
        {
            map.Column("Art_Artikeltyp");
            map.NotNullable(true);
        });
        Property(x => x.BasePrice, map => map.Column("Art_Grundpreis"));
        Property(x => x.ChangedOn, map =>
        {
            map.Column("Art_ChangedOn");
            map.Type(NHibernateUtil.DateTime);
            map.NotNullable(true);
        });
        Property(x => x.EanCode1, map => map.Column("Art_EAN1"));
        Property(x => x.EanCode2, map => map.Column("Art_EAN2"));
        Property(x => x.EanCode3, map => map.Column("Art_EAN3"));
        Property(x => x.EanCode4, map => map.Column("Art_EAN4"));
        Property(x => x.DivisionId, map => map.Column("Art_BereichID"));
        ManyToOne(x => x.Category, map =>
        {
            map.Column("Art_AKatID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Filling, map =>
        {
            map.Column("Art_AbfID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.BulkPackage, map =>
        {
            map.Column("Art_GrossID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Vat, map =>
        {
            map.Column("Art_MWSTID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Cycle, map =>
        {
            map.Column("Art_ZyklusID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.ECommerceGroup, map =>
        {
            map.Column("Art_EGruppeID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Region, map =>
        {
            map.Column("Art_RegID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.Country, map =>
        {
            map.Column("Art_LandID");
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.WineInfo, map =>
        {
            map.Column("Art_WeininfoID");
            map.Cascade(Cascade.None);
            map.NotFound(NotFoundMode.Ignore);
        });
        Bag(x => x.SalesPrices, colmap =>
        {
            colmap.Key(x =>
            {
                x.Column("VK_ArtID");
                x.ForeignKey("Art_ID");
            });
            colmap.Inverse(true);
        }, map => { map.OneToMany(); });
        Bag(x => x.StockMovements, colmap =>
        {
            colmap.Key(x =>
            {
                x.Column("Bew_ArtikelID");
                x.ForeignKey("Art_ID");
            });
            colmap.Inverse(true);
        }, map => { map.OneToMany(); });
        Bag(x => x.Stocks, colmap =>
        {
            colmap.Key(x =>
            {
                x.Column("Lagbest_ArtikelID");
                x.ForeignKey("Art_ID");
            });
            colmap.Inverse(true);
        }, map => { map.OneToMany(); });
    }
}