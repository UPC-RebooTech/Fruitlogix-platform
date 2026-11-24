using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;

public interface IDeliveryEventRepository : IBaseRepository<DeliveryEvent>
{
    /// <summary>Eventos de la entrega en orden cronológico.</summary>
    Task<IEnumerable<DeliveryEvent>> FindByDeliveryIdAsync(int deliveryId);
}
