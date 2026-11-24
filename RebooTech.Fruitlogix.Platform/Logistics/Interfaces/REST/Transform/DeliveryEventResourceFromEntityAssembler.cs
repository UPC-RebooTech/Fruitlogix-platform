using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;

public static class DeliveryEventResourceFromEntityAssembler
{
    public static DeliveryEventResource ToResourceFromEntity(DeliveryEvent entity) =>
        new(
            entity.Id,
            entity.DeliveryId,
            entity.Type.ToString(),
            entity.Name,
            entity.Note,
            entity.Latitude,
            entity.Longitude,
            entity.OccurredAt,
            entity.CreatedByUserId
        );
}
