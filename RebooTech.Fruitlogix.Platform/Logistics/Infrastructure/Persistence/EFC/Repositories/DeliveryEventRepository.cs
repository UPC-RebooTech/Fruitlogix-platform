using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Infrastructure.Persistence.EFC.Repositories;

public class DeliveryEventRepository(AppDbContext context)
    : BaseRepository<DeliveryEvent>(context), IDeliveryEventRepository
{
    public async Task<IEnumerable<DeliveryEvent>> FindByDeliveryIdAsync(int deliveryId) =>
        await Context.Set<DeliveryEvent>()
            .Where(e => e.DeliveryId == deliveryId)
            .OrderBy(e => e.OccurredAt)
            .ThenBy(e => e.Id)
            .ToListAsync();
}
