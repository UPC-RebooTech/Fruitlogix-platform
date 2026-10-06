using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Transform;

public static class CreateTrackingLogCommandFromResourceAssembler
{
    public static CreateTrackingLogCommand ToCommandFromResource(CreateTrackingLogResource resource) =>
        new(
            resource.DeliveryId,
            resource.Timestamp,
            resource.Latitude,
            resource.Longitude,
            resource.Temperature,
            resource.Humidity
        );
}