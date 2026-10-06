using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;

public interface ITrackingLogRepository : IBaseRepository<TrackingLog>
{
    Task<IEnumerable<TrackingLog>> FindByDeliveryIdAsync(int deliveryId);
}