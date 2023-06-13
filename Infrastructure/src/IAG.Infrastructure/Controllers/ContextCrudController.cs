using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Crud;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Controllers;

public class ContextCrudController<TKey, TEntity> : ControllerBase
    where TKey : IComparable
    where TEntity : class, IEntityKey<TKey>, new()
{
    protected DbContext Context { get; set; }

    [HttpPost]
    public async Task<ActionResult<TEntity>> Post([FromBody] TEntity item)
    {
        if (item == null)
        {
            return BadRequest();
        }

        Context.Set<TEntity>().Add(item);
        await Context.SaveChangesAsync();

        return Created(Request.Path + "/" + item.Id, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(TKey id, [FromBody] TEntity item)
    {
        if (item == null || !item.Id.Equals(id))
        {
            return BadRequest();
        }
        if (await Context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(t => t.Id.Equals(id)) == null)
        {
            return NotFound();
        }

        Context.Set<TEntity>().Update(item);
        await Context.SaveChangesAsync();

        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(TKey id)
    {
        var item = await Context.Set<TEntity>().SingleOrDefaultAsync(t => t.Id.Equals(id));
        if (item == null)
        {
            return NotFound();
        }
            
        Context.Set<TEntity>().Remove(item);
        await Context.SaveChangesAsync();

        return new NoContentResult();
    }

    [HttpGet]
    public ActionResult<IEnumerable<TEntity>> GetAll()
    {
        return Ok(Context.Set<TEntity>().AsNoTracking().ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TEntity>> Get(TKey id)
    {
        var item = await Context.Set<TEntity>().SingleOrDefaultAsync(t => t.Id.Equals(id));
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }
}