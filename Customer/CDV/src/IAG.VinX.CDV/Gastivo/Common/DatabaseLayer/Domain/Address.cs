using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Address
{
    public virtual int Id { get; set; }
    public virtual decimal? AddressNumber { get; set; }
    public virtual string SearchTerm { get; set; }
    public virtual string Name { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string Street { get; set; }
    public virtual string ZipCode { get; set; }
    public virtual string City { get; set; }
    public virtual string Company { get; set; }
    public virtual PriceGroup PriceGroup { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual bool TransmitToGastivo { get; set; }
    public virtual int? PaymentConditionId { get; set; }
    public virtual int? DeliveryConditionId { get; set; }
    public virtual short? PriceCondition { get; set; }
    public virtual Address ConditionAddress { get; set; }
    public virtual Address BillingAddress { get; set; }
    [CanBeNull] public virtual IList<SpecialPrice> SpecialPrices { get; set; }
}