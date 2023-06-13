using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Atlas;
using IAG.VinX.Smith.HelloTess.SyncLogic;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.HelloTess.Dto;

[UsedImplicitly]
[TablePrefix("ArtKat_")]
[Table("Artikelkategorie")]
public class VxArticleCategory : IKeyable
{
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public byte[] Guid { get; set; }

    public string Bezeichnung { get; set; }

    public string Key => new GuidConverter().ToBigEndianGuid(Guid).ToString();
}