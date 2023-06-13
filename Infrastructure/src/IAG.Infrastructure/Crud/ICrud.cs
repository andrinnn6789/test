using System;

namespace IAG.Infrastructure.Crud;

public interface ICrud<in TKey, TEntity>
    where TKey : IComparable
{
    void Delete(TKey id);

    TEntity Get(TKey id);

    void Insert(TEntity item);

    void Update(TEntity item);

    void Upsert(TEntity item);
}