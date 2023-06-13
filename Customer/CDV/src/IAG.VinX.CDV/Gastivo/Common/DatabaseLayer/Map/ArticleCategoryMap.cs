using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class ArticleCategoryMap: ClassMapping<ArticleCategory>
{
    public ArticleCategoryMap()
    {
        Schema("VinX");
        Table("Artikelkategorie");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("ArtKat_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("ArtKat_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.DescriptionFrench, map =>
        {
            map.Column("ArtKat_BezeichnungFR");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.DescriptionItalian, map =>
        {
            map.Column("ArtKat_BezeichnungIT");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.ArticleType, map => map.Column("ArtKat_Artikeltyp"));
    }
}