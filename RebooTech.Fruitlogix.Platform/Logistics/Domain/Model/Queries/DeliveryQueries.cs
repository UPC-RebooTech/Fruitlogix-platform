namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Queries;

public record GetDeliveryByIdQuery(int DeliveryId);

public record GetDeliveriesByOrderIdQuery(int OrderId);

public record GetDeliveryEventsQuery(int DeliveryId);
