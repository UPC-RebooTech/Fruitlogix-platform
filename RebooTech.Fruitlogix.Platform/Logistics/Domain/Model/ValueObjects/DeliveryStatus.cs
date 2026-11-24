namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;

public enum DeliveryStatus
{
    PendingDispatch,
    InTransit,
    Delayed,
    Delivered,
    Failed,
    /// <summary>Llegó al destino; la recepción del cliente la pasa a Delivered.</summary>
    Arrived
}