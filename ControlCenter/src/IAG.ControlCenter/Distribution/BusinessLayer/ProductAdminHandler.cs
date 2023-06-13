using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception.HttpException;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public class ProductAdminHandler : IProductAdminHandler
{
    private readonly DistributionDbContext _context;

    public ProductAdminHandler(DistributionDbContext context)
    {
        _context = context;
    }

    public Task<List<ProductInfo>> GetProductsAsync()
    {
        var mapper = new ProductInfoMapper();
            
        return _context.Products.AsNoTracking().Select(p => mapper.NewDestination(p)).ToListAsync();
    }

    public Task<List<ReleaseInfo>> GetReleasesAsync()
    {
        var mapper = new ReleaseInfoMapper();
            
        return _context.Releases.AsNoTracking()
            .Where(r => !r.Disabled)
            .Select(r => mapper.NewDestination(r))
            .ToListAsync();
    }

    public async Task<ProductInfo> RegisterProductAsync(ProductRegistration product)
    {
        if (string.IsNullOrEmpty(product?.ProductName))
        {
            throw new BadRequestException("ProductName is mandatory");
        }

        var productDb = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Name == product.ProductName);
        if (productDb == null)
        {
            productDb = new Product
            {
                Name = product.ProductName,
                ProductType = product.Type,
                DependsOnProductId = product.DependsOnProductId,
                Description = product.Description,
            };
            await _context.Products.AddAsync(productDb);
        }
        if (product.Type == ProductType.Updater)
        {
            foreach (var customer in _context.Customers.Include(c => c.ProductCustomers))
            {
                if (customer.ProductCustomers.Any(pc => pc.ProductId == productDb.Id))
                    continue;
                customer.ProductCustomers.Add(new ProductCustomer
                {
                    ProductId = productDb.Id,
                    CustomerId = customer.Id
                });
            }
        }
        await _context.SaveChangesAsync();

        return new ProductInfoMapper().NewDestination(productDb);
    }

    public async Task<ReleaseInfo> RegisterReleaseAsync(Guid productId, ReleaseRegistration release)
    {
        if (!await _context.Products.AnyAsync(p => p.Id == productId))
        {
            throw new NotFoundException("Product with ID '{0}' not found", productId);
        }
        if (release ==  null)
        {
            throw new BadRequestException("Release is mandatory");
        }
        if (string.IsNullOrEmpty(release.Platform))
        {
            throw new BadRequestException("Platform is mandatory");
        }

        var releaseDb = await _context.Releases.AsNoTracking().FirstOrDefaultAsync(
            r => r.ProductId == productId
                 && r.ReleaseVersion.ToUpper() == release.ReleaseVersion.ToUpper()
                 && r.Platform.ToUpper() == release.Platform.ToUpper());

        if (releaseDb == null)
        {
            releaseDb = new Release
            {
                ProductId = productId,
                ReleaseVersion = release.ReleaseVersion,
                Platform = release.Platform,
                Description = release.Description,
                ArtifactPath = release.ArtifactPath,
                ReleasePath = release.ReleasePath
            };
            _context.Releases.Add(releaseDb);
            await _context.SaveChangesAsync();
        }

        return new ReleaseInfoMapper().NewDestination(releaseDb);
    }

    public async Task<IEnumerable<FileMetaInfo>> AddFilesToReleaseAsync(Guid releaseId, IEnumerable<FileRegistration> files)
    {
        var releaseDb = await _context.Releases.AsNoTracking().FirstOrDefaultAsync(v => v.Id == releaseId);
        if (releaseDb == null)
        {
            throw new NotFoundException("Release with ID '{0}' not found", releaseId);
        }
        if (releaseDb.ReleaseDate.HasValue)
        {
            throw new BadRequestException("Cannot add files to approved release!");
        }

        var mapper = new FileInfoMapper();
        var result = new List<FileMetaInfo>();
        var fileStoreEntries = await _context.FileStores
            .AsNoTracking()
            .Select(f => new FileStore()
            {
                Id = f.Id,
                Name = f.Name,
                FileVersion = f.FileVersion,
                ProductVersion = f.ProductVersion,
                Checksum = f.Checksum,
                FileLastModifiedDate = f.FileLastModifiedDate,
                Data = f.Data != null ? Array.Empty<byte>() : null
            })
            .ToListAsync();
        foreach (var file in files)
        {
            var fileDb = FileCompareLogic.GetMatchingFiles(fileStoreEntries.AsQueryable(), file)
                .FirstOrDefault();
                
            var needFile = false;
            if (fileDb == null)
            {
                fileDb = new FileStore
                {
                    Name = file.Name,
                    FileVersion = file.FileVersion,
                    ProductVersion = file.ProductVersion,
                    Checksum = file.Checksum,
                    FileLastModifiedDate = file.FileLastModifiedDate
                };

                _context.FileStores.Add(fileDb);
                needFile = true;
            }
            else if (fileDb.Data == null)
            {
                needFile = true;
            }

            if (needFile)
            {
                var fileInfo = mapper.NewDestination(fileDb);
                result.Add(fileInfo);
            }

            var fileToRelease = await _context.ReleaseFileStores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FileStoreId == fileDb.Id && x.ReleaseId == releaseId);
            if (fileToRelease == null)
            {
                fileToRelease = new ReleaseFileStore()
                {
                    FileStoreId = fileDb.Id,
                    ReleaseId = releaseId
                };
                _context.ReleaseFileStores.Add(fileToRelease);
            }

            await _context.SaveChangesAsync();
        }

        return result;
    }

    public async Task<FileMetaInfo> SetFileContentAsync(Guid fileId, byte[] content)
    {
        var fileDb = await _context.FileStores.FirstOrDefaultAsync(f => f.Id == fileId);
        if (fileDb == null)
        {
            throw new NotFoundException("File with ID '{0}' not found", fileId);
        }
        if (fileDb.Data != null)
        {
            throw new BadRequestException("Content of file already set!");
        }

        using var md5 = MD5.Create();
        fileDb.Checksum = md5.ComputeHash(content);
        fileDb.Data = content;

        await _context.SaveChangesAsync();

        return new FileInfoMapper().NewDestination(fileDb);
    }

    public async Task<ReleaseInfo> ApproveReleaseAsync(Guid releaseId)
    {
        var releaseDb = await _context.Releases.FirstOrDefaultAsync(v => v.Id == releaseId);
        if (releaseDb == null)
        {
            throw new NotFoundException("Release with ID '{0}' not found", releaseId);
        }
        if (releaseDb.ReleaseDate != null)
        {
            throw new BadRequestException("Release already approved!");
        }
        if (await ReleaseHasFilesWithoutContentAsync(releaseId))
        {
            throw new BadRequestException("Not all files of the release have a content!");
        }

        releaseDb.ReleaseDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return new ReleaseInfoMapper().NewDestination(releaseDb);
    }

    public async Task RemoveReleaseAsync(Guid releaseId)
    {
        var releaseDb = await _context.Releases.FirstOrDefaultAsync(v => v.Id == releaseId);
        if (releaseDb == null)
        {
            throw new NotFoundException("Release with ID '{0}' not found", releaseId);
        }

        _context.ReleaseFileStores.RemoveRange(_context.ReleaseFileStores.Where(f => f.ReleaseId == releaseDb.Id));
        await _context.SaveChangesAsync();
        var orphanedFiles = _context.FileStores.Include(f => f.ReleaseFileStores).Where(f => !f.ReleaseFileStores.Any());
        _context.FileStores.RemoveRange(orphanedFiles);
        await _context.SaveChangesAsync();
        _context.Releases.Remove(releaseDb);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> ReleaseHasFilesWithoutContentAsync(Guid releaseId)
    {
        return await _context.ReleaseFileStores
            .Where(x => x.ReleaseId == releaseId)
            .Join(_context.FileStores, rf => rf.FileStoreId, f => f.Id, (_,f) => f.Data == null)
            .AnyAsync(x => x);
    }
}