using System;

namespace IAG.Infrastructure.DataLayer.Model.Base;

public interface IUniqueNamedEntity
{
    public string Name { get; }
}

public interface ITenantUniqueNamedEntity: IUniqueNamedEntity
{
    Guid? TenantId { get; }
}