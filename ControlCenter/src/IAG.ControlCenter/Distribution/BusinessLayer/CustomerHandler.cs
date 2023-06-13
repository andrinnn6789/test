using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception.HttpException;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public class CustomerHandler : ICustomerHandler
{
    private readonly DistributionDbContext _context;

    public CustomerHandler(DistributionDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerInfo> GetCustomerAsync(Guid customerId)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Include(c => c.ProductCustomers)
            .ThenInclude(x => x.Product)
            .Where(c => c.Id == customerId)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            throw new AuthenticationFailedException("Access to customer with ID '{0}' not allowed", customerId);
        }

        var mapper = new CustomerInfoMapper();
            
        return mapper.NewDestination(customer);
    }

    public async Task<IEnumerable<ProductInfo>> GetProductsAsync(Guid customerId)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Include(c => c.ProductCustomers)
            .ThenInclude(x => x.Product)
            .Where(c => c.Id == customerId)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            throw new AuthenticationFailedException("Access to customer with ID '{0}' not allowed", customerId);
        }

        var mapper = new ProductInfoMapper();
        var assignedProducts = customer.ProductCustomers
            .Select(x => mapper.NewDestination(x.Product))
            .ToList();
        var mainProductIds = assignedProducts
            .Where(p => p.ProductType == ProductType.IagService || p.ProductType == ProductType.Updater)
            .Select(p => p.Id).ToHashSet();
        var productSettings = await _context.Products
            .AsNoTracking()
            .Where(p => p.ProductType == ProductType.ConfigTemplate
                        && p.DependsOnProductId.HasValue && mainProductIds.Contains(p.DependsOnProductId.Value))
            .Select(p => mapper.NewDestination(p))
            .ToListAsync();

        return assignedProducts.Concat(productSettings);
    }

    public async Task<IEnumerable<ReleaseInfo>> GetReleasesAsync(Guid customerId, Guid productId)
    {
        var productWithReleases = await _context.Products
            .AsNoTracking()
            .Include(p => p.Releases)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (productWithReleases == null)
        {
            throw new AuthenticationFailedException("Access to product not allowed");
        }

        var accessCheckProductId = productId;
        if (productWithReleases.ProductType == ProductType.ConfigTemplate)
        {
            accessCheckProductId = productWithReleases.DependsOnProductId ?? Guid.Empty;
        }

        var hasAccess = await _context.ProductCustomers
            .AsNoTracking()
            .AnyAsync(x => x.CustomerId == customerId && x.ProductId == accessCheckProductId);
        if (!hasAccess)
        {
            throw new AuthenticationFailedException("Access to customer or product not allowed");
        }

        var releases = productWithReleases.Releases.Where(r => r.ReleaseDate.HasValue && !r.Disabled);
        var mapper = new ReleaseInfoMapper();
        return releases.Select(r => mapper.NewDestination(r));
    }

    public async Task<IEnumerable<FileMetaInfo>> GetReleaseFilesAsync(Guid customerId, Guid releaseId)
    {
        if (!await CheckReleaseAccessAsync(customerId, releaseId))
        {
            throw new AuthenticationFailedException("Access to customer or release not allowed");
        }

        var mapper = new FileInfoMapper();

        return await _context.ReleaseFileStores
            .AsNoTracking()
            .Include(rfs => rfs.FileStore)
            .Where(rfs => rfs.ReleaseId == releaseId)
            .Select(rfs => mapper.NewDestination(rfs.FileStore))
            .ToListAsync();
    }

    public async Task<FileWithDataInfo> GetFileAsync(Guid customerId, Guid fileId)
    {
        if (!await CheckFileAccessAsync(customerId, fileId))
        {
            throw new AuthenticationFailedException("Access to customer, release or file not allowed");
        }

        var file = await _context.FileStores.FirstAsync(f => f.Id == fileId);
        var fileWithData = new FileWithDataInfo()
        {
            Content = file.Data
        };

        new FileInfoMapper().UpdateDestination(fileWithData, file);
            
        return fileWithData;
    }

    public async Task<InstallationInfo> RegisterInstallationAsync(Guid customerId, InstallationRegistration installation)
    {
        if (!await CheckCustomerAccessAsync(customerId))
        {
            throw new AuthenticationFailedException("Access to installation not allowed");
        }

        if (string.IsNullOrEmpty(installation?.InstanceName))
        {
            throw new ArgumentException("InstanceName name is mandatory");
        }

        var installationDb = new Installation
        {
            CustomerId = customerId,
            ProductId = installation.ProductId,
            InstanceName = installation.InstanceName,
            ReleaseVersion = installation.ReleaseVersion,
            Platform = installation.Platform,
            Description = installation.Description
        };

        await _context.Installations.AddAsync(installationDb);
        await _context.SaveChangesAsync();

        return new InstallationInfo
        {
            Id = installationDb.Id,
            CustomerId = installationDb.CustomerId
        };
    }

    public async Task<IEnumerable<LinkInfo>> GetLinksAsync(Guid customerId)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer == null)
        {
            throw new AuthenticationFailedException("Access to customer with ID '{0}' not allowed", customerId);
        }

        var mapper = new LinkInfoMapper();

        return await _context.Links
            .AsNoTracking()
            .Select(l => mapper.NewDestination(l))
            .ToListAsync();
    }

    private Task<bool> CheckReleaseAccessAsync(Guid customerId, Guid releaseId)
    {
        var releases = _context.Releases
            .Where(r => r.Id == releaseId);

        return CheckReleaseAccessAsync(customerId, releases);
    }

    private Task<bool> CheckFileAccessAsync(Guid customerId, Guid fileId)
    {
        var releases = _context.Releases
            .Join(_context.ReleaseFileStores.Where(rfs => rfs.FileStoreId == fileId), 
                r => r.Id, rfs => rfs.ReleaseId, (r, rfs) => r);

        return CheckReleaseAccessAsync(customerId, releases);
    }

    private Task<bool> CheckCustomerAccessAsync(Guid customerId)
    {
        return _context.Customers.AnyAsync(c => c.Id == customerId);
    }

    private async Task<bool> CheckReleaseAccessAsync(Guid customerId, IQueryable<Release> releaseQuery)
    {
        var releases = await releaseQuery
            .AsNoTracking()
            .Include(r => r.Product)
            .ToListAsync();

        if (releases.Any(r => r.Product?.ProductType == ProductType.ConfigTemplate))
        {
            return true;
        }

        var productIds = releases.Select(r => r.ProductId).ToHashSet();
        return await _context.ProductCustomers
            .AnyAsync(x => productIds.Contains(x.ProductId) && x.CustomerId == customerId);
    }
}