using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Infrastructure.Persistence.EFC.Repositories;

public class TrackingLogRepository : BaseRepository<TrackingLog>, ITrackingLogRepository
{
    public TrackingLogRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TrackingLog>> FindByDeliveryIdAsync(int deliveryId)
    {
        return await Context.Set<TrackingLog>()
            .Where(t => t.DeliveryId == deliveryId)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }
}