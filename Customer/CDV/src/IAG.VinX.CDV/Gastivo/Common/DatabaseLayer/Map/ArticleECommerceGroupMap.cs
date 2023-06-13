using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class ArticleECommerceGroupMap : ClassMapping<ArticleECommerceGroup>
{
    public ArticleECommerceGroupMap()
    {
        Schema("VinX");
        Table("ArtikelEGruppe");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("ArtEGrp_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Description, map =>
        {
            map.Column("ArtEGrp_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
        });
    }
}