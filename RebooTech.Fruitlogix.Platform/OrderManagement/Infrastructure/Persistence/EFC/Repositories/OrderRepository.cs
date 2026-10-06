using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Infrastructure.Persistence.EFC.Repositories;

public class OrderRepository(AppDbContext context)
    : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetAllOrdersWithItemsAsync() =>
        await Context.Set<Order>()
            .Include(o => o.Items)
            .ToListAsync();

    public async Task<Order?> GetOrderByIdWithItemsAsync(int id) =>
        await Context.Set<Order>()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> FindByClientIdAsync(int clientId) =>
        await Context.Set<Order>()
            .Where(o => o.CommercialClientId == clientId)
            .Include(o => o.Items)
            .ToListAsync();

    public async Task<IEnumerable<Order>> FindByProducerIdAsync(int producerId)
    {
        var orders = await Context.Set<Order>()
            .Include(o => o.Items)
            .ToListAsync();
    
        return orders.Where(o => o.ProducerId != null && o.ProducerId.Value == producerId);
    }
}