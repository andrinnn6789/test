using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Crud;
using IAG.Infrastructure.Exception.HttpException;

namespace IAG.Infrastructure.Test.Controllers;

public class CrudString : ICrud<string, StringKey>
{
    private readonly List<StringKey> _data = new();
        
    public void Delete(string id)
    {
        var item = _data.FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id);
        }

        _data.Remove(item);
    }

    public StringKey Get(string id)
    {
        var item = _data.FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id);
        }

        return item;
    }

    public void Insert(StringKey item)
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        if (_data.Any(t => t.Id == item.Id))
        {
            throw new DuplicateKeyException(item.Id);
        }

        _data.Add(item);
    }

    public void Update(StringKey item)
    {
        var itemUpd = _data.FirstOrDefault(t => t.Id == item.Id);
        if (itemUpd == null)
        {
            throw new NotFoundException(item.Id);
        }

        _data.Remove(itemUpd);
        _data.Add(item);
    }

    public void Upsert(StringKey item)
    {
        var itemUpd = _data.FirstOrDefault(t => t.Id == item.Id);
        if (itemUpd != null)
            _data.Remove(itemUpd);
        _data.Add(item);
    }
}