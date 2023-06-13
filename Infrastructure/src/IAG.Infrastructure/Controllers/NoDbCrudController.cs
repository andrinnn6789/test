using System;

using IAG.Infrastructure.Crud;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Controllers;

public class NoDbCrudController<TKey, TEntity, TCrud> : ControllerBase
    where TKey : IComparable
    where TEntity : IEntityKey<TKey>
    where TCrud : ICrud<TKey, TEntity>
{
    protected TCrud Crud { get; set; }

    [HttpPost]
    public IActionResult Create([FromBody] TEntity item)
    {
        if (item == null)
        {
            return BadRequest();
        }

        Crud.Insert(item);

        return Created(Request.Path + "/" + item.Id, item);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(TKey id)
    {
        Crud.Delete(id);

        return new NoContentResult();
    }

    [HttpGet("{id}")]
    public ActionResult<TEntity> GetById(TKey id)
    {
        return Crud.Get(id);
    }

    [HttpPut("{id}")]
    public IActionResult Update(TKey id, [FromBody] TEntity item)
    {
        if (item == null || !item.Id.Equals(id))
        {
            return BadRequest();
        }

        Crud.Update(item);

        return new NoContentResult();
    }
}