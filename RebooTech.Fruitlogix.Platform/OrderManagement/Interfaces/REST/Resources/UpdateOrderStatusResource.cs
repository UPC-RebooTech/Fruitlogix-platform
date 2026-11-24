using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

/// <summary>Decisión del productor sobre un pedido.</summary>
public enum OrderStatusAction
{
    /// <summary>Pending → InPreparation.</summary>
    Accept,
    /// <summary>Pending o InPreparation → Rejected (requiere reason).</summary>
    Reject
}

/// <param name="Action">accept o reject (sin distinguir mayúsculas).</param>
/// <param name="Reason">Obligatorio para reject.</param>
public record UpdateOrderStatusResource(
    [property: SwaggerEnumValues(typeof(OrderStatusAction))] string Action,
    string? Reason = null
);
