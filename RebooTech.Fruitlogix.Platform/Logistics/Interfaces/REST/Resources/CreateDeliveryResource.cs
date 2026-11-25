namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

public record CreateDeliveryResource(
    int      OrderId,
    string?  DriverName,
    string?  DriverPhone,
    string?  VehiclePlate,
    string?  VehicleType,
    string   RouteOrigin,
    string   RouteDestination,
    double   RouteDistanceKm,
    DateTime EstimatedTimeOfArrival,
    int?     DistributorUserId = null,
    // Flota (opcional): si se envían, nombre/teléfono y placa/tipo se toman del chofer y del vehículo.
    int?     DriverId = null,
    int?     VehicleId = null
);