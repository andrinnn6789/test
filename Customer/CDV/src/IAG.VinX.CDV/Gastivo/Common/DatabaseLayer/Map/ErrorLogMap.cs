using System.Diagnostics.CodeAnalysis;

using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public class ErrorLogMap : ClassMapping<ErrorLog>
{
    public ErrorLogMap()
    {
        Schema("VinX");
        Table("Fehlerprotokoll");
        Lazy(true);
        Id(x => x.Id, map =>
        {
            map.Column("Fehlerprot_ID");
            map.Generator(Generators.Identity);
        });
        Property(x => x.Title, map =>
        {
            map.Column("Fehlerprot_Bezeichnung");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.Occurence, map =>
        {
            map.Column("Fehlerprot_Aufgetreten");
            map.Type(NHibernateUtil.AnsiString);
        });
        Property(x => x.LogDate, map =>
        {
            map.Column("Fehlerprot_DatumLog");
            map.Type(NHibernateUtil.DateTime);
        });
        Property(x => x.DateMillisecond, map =>
        {
            map.Column("Fehlerprot_DatumMS");
        });
        Property(x => x.Description, map =>
        {
            map.Column("Fehlerprot_Meldung");
            map.Type(NHibernateUtil.Binary);
            map.Length(int.MaxValue);
        });
        Property(x => x.User, map =>
        {
            map.Column("Fehlerprot_User");
            map.Type(NHibernateUtil.AnsiString);
        });
    }
}