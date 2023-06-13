using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// simple price for an article and price group
/// </summary>
[DataContract]
[DisplayName("Price")]
[TableCte(@"
        WITH Sales AS 
		(
        SELECT 
           VK_ID AS SalesId, 
           AktTyp_Typ AS PromotionKind, 
		   IsNull(ABS(AktTyp_MRabattErlaubt), 0) AS PromotionQuantityDiscountAllowed, 
		   VK_Aktionstext AS PromotionText, 
		   VK_PreisAktion AS PromotionPrice, 
		   AktTyp_Rabatt AS PromotionPercent, 
           AktTyp_CentOff AS PromotionCentOff, 
		   AktTyp_AnzahlAbzug AS PromotionReducePce, 
		   AktTyp_MengeFuerAbzug AS PromotionReducedPcePer, 
		   VK_DatumVon AS PromotionValidFrom, 
		   VK_DatumBis AS PromotionValidUntil
        FROM VKPreis
        LEFT OUTER JOIN Aktionstyp ON AktTyp_Id = VK_AktionstypID
        WHERE VK_IstAktion = -1 AND IsNull(VK_DatumBis, Today()) >= Today()
        ),
        PriceGw AS 
		(
        SELECT 
           VK_ArtID AS ArticleId, 
           KundPreis_Bezeichnung AS PriceGroupName,
		   VK_Preis AS UnitPrice, 
		   ABS(IsNull(VK_IstNetto, 0)) AS IsNetto, 
           IsNull(PromotionKind, 0) AS PromotionKind, 
		   PromotionQuantityDiscountAllowed, PromotionText, PromotionPrice, PromotionPercent, 
           PromotionCentOff, PromotionReducePce, PromotionReducedPcePer, PromotionValidFrom, PromotionValidUntil,
           ROW_NUMBER() OVER (PARTITION BY VK_ArtID, VK_KGrpPreisID ORDER BY IsNull(VK_VonDatum, today()) DESC) AS RowNum,
           VK_ChangedOn AS ChangedOn
        FROM VKPreis
        JOIN Artikel ON Art_Id = VK_ArtID
        LEFT OUTER JOIN KundengruppePreis ON VK_KGrpPreisID = KundPreis_ID
        LEFT OUTER JOIN Sales ON VK_Id = SalesId        
        WHERE " + ArticleGw.MasterFilter + @"
        )
    ")]
public class PriceGw
{ 
	/// <summary>
	/// internal for join operation
	/// </summary>
	public int ArticleId { get; set; }
 
	/// <summary>
	/// timestamp utc last change, output
	/// </summary>
	public DateTime ChangedOn { get; set; }

	/// <summary>
	/// name of the price group, relates to the same attribute of a contact
	/// </summary>
	[DataMember(Name="priceGroupName")]
	public string PriceGroupName { get; set; }

	/// <summary>
	/// price excl. vat / VK_Preis
	/// </summary>
	[DataMember(Name="unitPrice")]
	public decimal? UnitPrice { get; set; }

	/// <summary>
	/// price is netto / VK_IstNetto
	/// </summary>
	[DataMember(Name="isNetto")]
	public bool IsNetto { get; set; }

	/// <summary>
	/// Gets or Sets PromotionKind
	/// </summary>
	[DataMember(Name="promotionKind")]
	public PromotionKindTypeGw PromotionKind { get; set; }

	/// <summary>
	/// quantity discount is allowed for this promotion / AktTyp_MRabattErlaubt
	/// </summary>
	[DataMember(Name="promotionQuantityDiscountAllowed")]
	public bool PromotionQuantityDiscountAllowed { get; set; }

	/// <summary>
	/// description of the promotion / VK_Aktionstext
	/// </summary>
	[DataMember(Name="promotionText")]
	public string PromotionText { get; set; }

	/// <summary>
	/// kind &#x3D; reducePrice, promotion price excl. vat / VK_PreisAktion
	/// </summary>
	[DataMember(Name="promotionPrice")]
	public decimal? PromotionPrice { get; set; }

	/// <summary>
	/// kind &#x3D; percent, reduction of the price in percent / AktTyp_Rabatt
	/// </summary>
	[DataMember(Name="promotionPercent")]
	public decimal? PromotionPercent { get; set; }

	/// <summary>
	/// kind &#x3D; centOff, reduction of the price in cents / AktTyp_CentOff
	/// </summary>
	[DataMember(Name="promotionCentOff")]
	public decimal? PromotionCentOff { get; set; }

	/// <summary>
	/// kind &#x3D; reducePce, reduction of billed pieces / AktTyp_AnzahlAbzug
	/// </summary>
	[DataMember(Name="promotionReducePce")]
	public int? PromotionReducePce { get; set; }

	/// <summary>
	/// kind &#x3D; centOff, number of units to apply promotionReducePce / AktTyp_MengeFueAbzug
	/// </summary>
	[DataMember(Name="promotionReducedPcePer")]
	public int? PromotionReducedPcePer { get; set; }

	/// <summary>
	/// valid from / VK_VonDatum
	/// </summary>
	[DataMember(Name="promotionValidFrom")]
	public DateTime? PromotionValidFrom { get; set; }

	/// <summary>
	/// valid until / VK_BisDatum
	/// </summary>
	[DataMember(Name="promotionValidUntil")]
	public DateTime? PromotionValidUntil { get; set; }
}