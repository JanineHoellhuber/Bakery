using Bakery.Core.Contracts;
using Bakery.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bakery.Persistence
{
  public class OrderItemRepository : IOrderItemRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public OrderItemRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<int> GetCountAsync()
    {
      return await _dbContext.OrderItems.CountAsync();
    }

        public async Task AddRangeAsync(IEnumerable<OrderItem> orderItems)
        {
            await _dbContext.OrderItems.AddRangeAsync(orderItems);
        }

        public async Task AddAsync(OrderItem orderItem)
        {
            await _dbContext.OrderItems.AddAsync(orderItem);
        }

    }
}
