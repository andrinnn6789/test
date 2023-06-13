using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class OnlineOrder
{
    public virtual int Id { get; set; }
    public virtual string OrderReference { get; set; }
    public virtual DateTime? OrderDate { get; set; }
    public virtual DateTime? DeliveryDateRequested { get; set; }
    public virtual string Hint { get; set; }
    [CanBeNull] public virtual Address OrderingAddress { get; set; }
    [CanBeNull] public virtual Address DeliveryAddress { get; set; }
    [CanBeNull] public virtual Address ConditionAddress { get; set; }
    public virtual int? DivisionId { get; set; }
    public virtual int? DeliveryConditionId { get; set; }
    public virtual int? PaymentConditionId { get; set; }
    public virtual int? ProviderId { get; set; }
    public virtual short? IsVatIncluded { get; set; }
    public virtual short? IsProcessed { get; set; }
    public virtual short? NumberOfLines { get; set; }
    public virtual IList<OnlineOrderLine> OnlineOrderLines { get; set; }
}