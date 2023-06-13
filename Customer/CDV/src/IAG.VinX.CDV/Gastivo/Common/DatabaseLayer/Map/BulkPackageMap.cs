using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class BulkPackageMap: ClassMapping<BulkPackage>
{
    public BulkPackageMap()
    {
        Schema("VinX");
        Table("Grossgebinde");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Gross_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.SearchTerm, map =>
        {
            map.Column("Gross_Suchbegriff");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.Abbreviation, map =>
        {
            map.Column("Gross_Kuerzel");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.QuantityPerBulkPackage, map =>
        {
            map.Column("Gross_EinhProGG");
            map.NotNullable(true);
        });
        Property(x => x.BreakageAllowed, map => map.Column("Gross_AnbruchErlaubt"));
        Property(x => x.IsPallet, map => map.Column("Gross_Palettiert"));
        Property(x => x.BulkPackagePerPallet, map => map.Column("Gross_GebindeProPalette"));
        Property(x => x.DescriptionForWeb, map =>
        {
            map.Column("Gross_BezeichnungWeb");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.DescriptionForWebItalian, map =>
        {
            map.Column("Gross_BezeichnungWebIT");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.DescriptionForWebFrench, map =>
        {
            map.Column("Gross_BezeichnungWebFR");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.AbbreviationForWeb, map =>
        {
            map.Column("Gross_KuerzelWeb");
            map.Type(NHibernateUtil.AnsiString);
        });
        ManyToOne(x => x.BulkPackageArticle, map => 
        {
            map.Column("Gross_ArtID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.BreakageArticle, map => 
        {
            map.Column("Gross_AnbruchArtikelID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
        ManyToOne(x => x.PalletArticle, map => 
        {
            map.Column("Gross_PaletteArtikelID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
    }
}
