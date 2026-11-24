namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

public record GetDeliveryByIdQuery(int DeliveryId);

public record GetDeliveriesByOrderIdQuery(int OrderId);

public record GetDeliveryEventsQuery(int DeliveryId);

/// <summary>Llegadas (eventos Arrived), más recientes primero.</summary>
public record GetDeliveryArrivalsQuery(DateTimeOffset? Since);
