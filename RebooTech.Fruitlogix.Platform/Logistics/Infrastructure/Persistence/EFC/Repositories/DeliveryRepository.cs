using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Infrastructure.Persistence.EFC.Repositories;

public class DeliveryRepository : BaseRepository<Delivery>, IDeliveryRepository
{
    public DeliveryRepository(AppDbContext context) : base(context) { }
}