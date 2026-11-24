using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

/// <summary>Entrega. currentStatus: PendingDispatch → InTransit (start-dispatch) → Delayed (report-delay, opcional) → Arrived (arrive) → Delivered (recepción del pedido). Failed queda reservado.</summary>
public record DeliveryResource(
    int      Id,
    int      OrderId,
    string   DriverName,
    string   DriverPhone,
    string   VehiclePlate,
    string   VehicleType,
    string   RouteOrigin,
    string   RouteDestination,
    double   RouteDistanceKm,
    DateTime EstimatedTimeOfArrival,
    [property: SwaggerEnumValues(typeof(DeliveryStatus))]
    string   CurrentStatus,
    DateTimeOffset? DispatchedAt,
    DateTimeOffset? ArrivedAt,
    DateTimeOffset? DeliveredAt
);