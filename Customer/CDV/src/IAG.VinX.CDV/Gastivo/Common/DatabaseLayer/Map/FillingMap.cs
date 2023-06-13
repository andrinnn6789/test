using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class FillingMap: ClassMapping<Filling>
{
    public FillingMap()
    {
        Schema("VinX");
        Table("Abfuellung");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Abf_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.SearchTerm, map =>
        {
            map.Column("Abf_Suchbegriff");
            map.Type(NHibernateUtil.AnsiString);
            map.NotNullable(true);
        });
        Property(x => x.Abbreviation, map =>
        {
            map.Column("Abf_Kuerzel");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.ContentInCl, map => map.Column("Abf_InhaltInCl"));
        Property(x => x.DescriptionForWeb, map =>
        {
            map.Column("Abf_BezeichnungWeb");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.AbbreviationForWeb, map =>
        {
            map.Column("Abf_KuerzelWeb");
            map.Type(NHibernateUtil.AnsiString);
        });
        ManyToOne(x => x.Article, map =>
        {
            map.Column("Abf_ArtID");
            map.NotNullable(true);
            map.Cascade(Cascade.None);
        });
    }
}
