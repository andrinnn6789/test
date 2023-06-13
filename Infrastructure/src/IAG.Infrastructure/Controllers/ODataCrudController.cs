using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Crud;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Controllers;

[ApiController]
public class ODataCrudController<TKey, TEntity> : ODataController
    where TKey : IComparable
    where TEntity : class, IEntityKey<TKey>, new()
{
    private readonly DbContext _context;

    protected ODataCrudController(DbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> Post([FromBody] TEntity item)
    {
        _context.Set<TEntity>().Add(item);
        await _context.SaveChangesAsync();

        return new CreatedResult(string.Empty, item);
    }

    [HttpPut("{key}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> Put(TKey key, [FromBody] TEntity item)
    {
        if (!item.Id.Equals(key))
        {
            return BadRequest();
        }

        if (await _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(t => t.Id.Equals(key)) == null)
        {
            return NotFound();
        }

        _context.Set<TEntity>().Update(item);
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }

    [HttpPatch("{key}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> Patch(TKey key, [FromBody] Delta<TEntity> patch)
    {
        var item = await _context.Set<TEntity>().SingleOrDefaultAsync(t => t.Id.Equals(key));
        if (item == null)
        {
            return NotFound();
        }

        patch.Patch(item);
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }

    [HttpDelete("{key}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Delete)]
    public async Task<IActionResult> Delete(TKey key)
    {
        var item = await _context.Set<TEntity>().SingleOrDefaultAsync(t => t.Id.Equals(key));
        if (item == null)
        {
            return NotFound();
        }
            
        _context.Set<TEntity>().Remove(item);
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet]
    [ClaimAuthorization(
        ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read,
        ScopeNamesInfrastructure.ReaderScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read
    )]
    public ActionResult<IQueryable<TEntity>> Get()
    {
        return new(_context.Set<TEntity>().AsNoTracking().AsQueryable());
    }

    [HttpGet("{key}")]
    [ClaimAuthorization(
        ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read,
        ScopeNamesInfrastructure.ReaderScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read
    )]
    public ActionResult<TEntity> Get(TKey key)
    {
        var entry = _context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefault(t => t.Id.Equals(key));
        if (entry == null)
        {
            return NotFound();
        }

        return Ok(entry);
    }
}