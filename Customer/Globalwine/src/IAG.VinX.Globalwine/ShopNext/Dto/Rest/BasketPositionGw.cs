using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// order position of the basket
/// </summary>
[DisplayName("BasketPosition")]
[DataContract]
public class BasketPositionGw : PositionBaseGw
{
    public BasketPositionGw()
    {
        PosType = OrderPositionTypeGw.OrderPos;
    }

    /// <summary>
    /// available quantity in stock
    /// </summary>
    [DataMember(Name="stockQuantity")]
    public decimal StockQuantity { get; set; }

    /// <summary>
    /// list of the charges of the position, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "charges")]
    public IList<ChargePositionGw> Charges { get; set; } = new List<ChargePositionGw>();

    /// <summary>
    /// list of the discounts of the position, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "discounts")]
    public IList<DiscountPositionGw> Discounts { get; set; } = new List<DiscountPositionGw>();

    /// <summary>
    /// list of the packages of the position, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "packages")]
    public IList<PackagePositionGw> Packages { get; set; } = new List<PackagePositionGw>();

    /// <summary>
    /// total amount charges, output
    /// </summary>
    [DataMember(Name = "chargeTotalAmount")]
    public decimal ChargeTotalAmount { get; set; }

    /// <summary>
    /// total amount discounts, output
    /// </summary>
    [DataMember(Name = "discountTotalAmount")]
    public decimal DiscountTotalAmount { get; set; }

    /// <summary>
    /// total amount packages, output
    /// </summary>
    [DataMember(Name = "packagesTotalAmount")]
    public decimal PackagesTotalAmount { get; set; }
}