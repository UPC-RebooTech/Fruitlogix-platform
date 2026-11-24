namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;

/// <summary>Marca la llegada al destino (InTransit o Delayed → Arrived).</summary>
public record ArriveDeliveryCommand(
    int DeliveryId,
    int? ActorUserId,
    double? Latitude = null,
    double? Longitude = null,
    string? Note = null
);

/// <summary>Cambia conductor y/o vehículo. Los campos null conservan el valor actual.</summary>
public record UpdateDeliveryAssignmentCommand(
    int DeliveryId,
    string? DriverName,
    string? DriverPhone,
    string? VehiclePlate,
    string? VehicleType
);

/// <summary>Registra un punto de control con nombre en la ruta.</summary>
public record AddDeliveryCheckpointCommand(
    int DeliveryId,
    int? ActorUserId,
    string Name,
    double? Latitude = null,
    double? Longitude = null,
    string? Note = null,
    DateTimeOffset? OccurredAt = null
);
