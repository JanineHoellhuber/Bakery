using Bakery.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
  public interface IOrderItemRepository
  {
    Task<int> GetCountAsync();
    Task AddRangeAsync(IEnumerable<OrderItem> orderItems);

    Task AddAsync(OrderItem orderItem);
  }
}