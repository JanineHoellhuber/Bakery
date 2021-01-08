using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
  public interface IProductRepository
  {
    Task<int> GetCountAsync();
    Task AddRangeAsync(IEnumerable<Product> products);
     Task<Product[]> GetAllAsync();

        Task<Product> GetProductByIdAsync(int id);
        Task AddAsync(Product product);

        Task<IEnumerable<ProductDto>> GetWithFilterAsync(double priceFrom, double priceTo);

    }
}