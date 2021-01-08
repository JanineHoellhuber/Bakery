using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Persistence
{
  public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Products.CountAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            await _dbContext.Products.AddRangeAsync(products);
        }

        public async Task<Product[]> GetAllAsync()
        {
            return await _dbContext.Products.ToArrayAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _dbContext.Products.Where(p => p.Id == id).SingleOrDefaultAsync();
        }

        public async Task AddAsync(Product product)
        {
             await _dbContext.Products.AddAsync(product);
        }
        public async Task<IEnumerable<ProductDto>> GetWithFilterAsync(double priceFrom, double priceTo)
        {
            var query = _dbContext.Products.Where(p => p.Price >= priceFrom)
                .AsQueryable();

            if (priceTo > 0)
            {
                query = query.Where(p => p.Price <= priceTo);
            }

            return await query
                .Include(p => p.OrderItems)
                .Select(p => new ProductDto(p))
                .ToArrayAsync();
        }

    }
}
