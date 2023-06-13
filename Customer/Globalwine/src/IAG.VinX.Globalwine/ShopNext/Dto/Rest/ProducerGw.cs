using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// details about the producers
/// </summary>
[DataContract]
[DisplayName("Producer")]
[TableCte(@"
        WITH ProducerGw (
            Id, HasActiveProducts, Name, Description, 
            History, Homepage, OwnProduction, ChangedOn
        ) AS (
        SELECT 
            Prod_Id, Sign(Count(Art_Id)), Prod_Bezeichnung, Prod_Text,
            Prod_Geschichte, Prod_Homepage, ABS(Prod_EigenProduktion), Prod_ChangedOn
        FROM Produzent
        LEFT OUTER JOIN Artikel ON Prod_Id = Art_ProdID
        WHERE IsNull(Art_Aktiv, -1) = -1 AND IsNull(Art_OnlineAktiv, -1) = -1
        GROUP BY 
            Prod_Id, Prod_Bezeichnung, Prod_Text,
            Prod_Geschichte, Prod_Homepage, ABS(Prod_EigenProduktion), Prod_ChangedOn
        )
    ")]
public class ProducerGw
{ 
    /// <summary>
    /// id in VinX / Prod_Id
    /// </summary>
    [DataMember(Name="id")]
    public int? Id { get; set; }

    /// <summary>
    /// calculated field: true if there are any active articles for the shop, else false
    /// </summary>
    [DataMember(Name="hasActiveProducts")]
    public bool HasActiveProducts { get; set; }

    /// <summary>
    /// timestamp utc last change
    /// </summary>
    [DataMember(Name="changedOn")] 
    public DateTime ChangedOn { get; set; }

    /// <summary>
    /// name of the producer / Prod_Bezeichnung
    /// </summary>
    [DataMember(Name="name")]
    public string Name { get; set; }

    /// <summary>
    /// description of the producer / Prod_Text
    /// </summary>
    [DataMember(Name="description")]
    public string Description { get; set; }

    /// <summary>
    /// history of the producer / Prod_Geschichte
    /// </summary>
    public string History {get; set; }
    [NotMapped]
    [DataMember(Name="history")]
    public string HistoryClean => RtfCleaner.Clean(History);

    /// <summary>
    /// homepage / Prod_Homepage
    /// </summary>
    [DataMember(Name="homepage")]
    public string Homepage { get; set; }

    /// <summary>
    /// own production
    /// </summary>
    [DataMember(Name="ownProduction")]
    public bool OwnProduction { get; set; }
}