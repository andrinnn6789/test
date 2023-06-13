using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class WineInfoMap : ClassMapping<WineInfo>
{
    public WineInfoMap()
    {
        Schema("VinX");
        Table("Weininfo");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Wein_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Character, map =>
        {
            map.Column("Wein_Charakter");
            map.Type(NHibernateUtil.AnsiString);
            map.Length(64000);
        });
        Property(x => x.CharacterFrench, map =>
        {
            map.Column("Wein_CharakterFR");
            map.Type(NHibernateUtil.AnsiString);
            map.Length(64000);
        });
        Property(x => x.CharacterItalian, map =>
        {
            map.Column("Wein_CharakterIT");
            map.Type(NHibernateUtil.AnsiString);
            map.Length(64000);
        });
        Bag(x => x.Recommendations, colmap =>
        {
            colmap.Table("ArtikelEmpfehlung");
            colmap.Cascade(Cascade.None);
            colmap.Key(k => k.Column("ArtZus_WeininfoID"));
            colmap.Inverse(true);
        }, map => map.ManyToMany(p => p.Column("ArtZus_EmpfehlungID")));
        Bag(x => x.Grapes, colmap =>
        {
            colmap.Table("ArtikelZusammensetzung");
            colmap.Cascade(Cascade.None);
            colmap.Key(k => k.Column("ArtZus_WeininfoID"));
            colmap.Inverse(true);
        }, map => map.ManyToMany(p => p.Column("ArtZus_TraubensorteID")));
    }
}