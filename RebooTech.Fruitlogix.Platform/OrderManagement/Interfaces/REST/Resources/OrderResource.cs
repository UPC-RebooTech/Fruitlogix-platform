using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

public record OrderResource(
    int Id,
    int CommercialClientId,
    int? ProducerId,
    [property: SwaggerEnumValues(typeof(OrderStatus))]
    string Status,
    DateOnly DeliveryDueDate,
    string DeliveryAddress,
    decimal TotalAmount,
    string? Notes,
    string? CancellationReason,
    List<OrderItemResource> Items,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? DeliveredAt,
    // Alias de DeliveredAt (la app acepta cualquiera de los dos).
    DateTimeOffset? ReceivedAt,
    OrderReceptionResource? Reception,
    int? BatchId,
    string? ProducerName,
    // Razón social del cliente (perfil) o su username.
    string? CommercialClientName
);