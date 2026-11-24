using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
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

    public async Task<IEnumerable<DeliveryEvent>> FindByTypeAsync(DeliveryEventType type, DateTimeOffset? since)
    {
        var events = Context.Set<DeliveryEvent>().Where(e => e.Type == type);
        if (since is { } from) events = events.Where(e => e.OccurredAt >= from);
        return await events.OrderByDescending(e => e.OccurredAt).ThenByDescending(e => e.Id).ToListAsync();
    }
}
