using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;

namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

/// <summary>Evento del historial de una entrega.</summary>
public record DeliveryEventResource(
    int Id,
    int DeliveryId,
    [property: SwaggerEnumValues(typeof(DeliveryEventType))] string Type,
    string? Name,
    string? Note,
    double? Latitude,
    double? Longitude,
    DateTimeOffset OccurredAt,
    int? CreatedByUserId
);

/// <summary>Body opcional de POST /deliveries/{id}/arrive.</summary>
public record ArriveDeliveryResource(double? Latitude = null, double? Longitude = null, string? Note = null);

/// <summary>Cambio de asignación. Los campos que no se envían conservan su valor.</summary>
public record UpdateDeliveryAssignmentResource(
    string? DriverName = null,
    string? DriverPhone = null,
    string? VehiclePlate = null,
    string? VehicleType = null
);

/// <summary>Punto de control con nombre. occurredAt por defecto es ahora.</summary>
public record AddDeliveryCheckpointResource(
    string Name,
    double? Latitude = null,
    double? Longitude = null,
    string? Note = null,
    DateTimeOffset? OccurredAt = null
);
