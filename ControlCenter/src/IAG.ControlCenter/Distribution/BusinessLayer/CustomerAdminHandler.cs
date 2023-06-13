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

public class CustomerAdminHandler : ICustomerAdminHandler
{
    private readonly DistributionDbContext _context;

    public CustomerAdminHandler(DistributionDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerInfo> RegisterCustomerAsync(CustomerRegistration customer)
    {
        if (string.IsNullOrEmpty(customer?.CustomerName))
        {
            throw new BadRequestException("Customer is mandatory");
        }
        if (customer.CustomerId == Guid.Empty)
        {
            throw new BadRequestException("CustomerId is mandatory");
        }

        var customerDb = await _context.Customers
            .AsNoTracking()
            .Include(c => c.ProductCustomers)
            .FirstOrDefaultAsync(c => c.Id == customer.CustomerId);
        if (customerDb == null)
        {
            customerDb = new Customer()
            {
                Id = customer.CustomerId,
                Name = customer.CustomerName,
                CustomerCategoryId = customer.CustomerCategoryId,
                Description = customer.Description,
            };
            _context.Customers.Add(customerDb);
            await _context.SaveChangesAsync();
        }

        return new CustomerInfoMapper().NewDestination(customerDb);
    }

    public async Task<IEnumerable<CustomerInfo>> GetCustomersAsync()
    {
        var mapper = new CustomerInfoMapper();

        return await _context.Customers
            .AsNoTracking()
            .Include(c => c.ProductCustomers)
            .Select(c => mapper.NewDestination(c)).ToListAsync();
    }

    public async Task AddProductsAsync(Guid customerId, IEnumerable<Guid> productsToAdd)
    {
        if (customerId == Guid.Empty)
        {
            throw new BadRequestException("CustomerId is mandatory");
        }
        if (productsToAdd == null)
        {
            throw new BadRequestException("ProductsToAdd is mandatory");
        }

        var existingProductIds = await _context.ProductCustomers
            .AsNoTracking()
            .Where(pc => pc.CustomerId == customerId)
            .Select(pc => pc.ProductId)
            .ToListAsync();

        foreach (var productId in productsToAdd.Where(p => !existingProductIds.Contains(p)))
        {
            _context.ProductCustomers.Add(new ProductCustomer() {CustomerId = customerId, ProductId = productId});
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveProductsAsync(Guid customerId, IEnumerable<Guid> productsToRemove)
    {
        if (customerId == Guid.Empty)
        {
            throw new BadRequestException("CustomerId is mandatory");
        }
        if (productsToRemove == null)
        {
            throw new BadRequestException("ProductsToRemove is mandatory");
        }

        var productAssignmentsToRemove = await _context.ProductCustomers
            .Where(pc => pc.CustomerId == customerId)
            .ToListAsync();

        _context.ProductCustomers.RemoveRange(productAssignmentsToRemove);

        await _context.SaveChangesAsync();
    }
}