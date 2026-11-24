namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;

/// <summary>
///     Marca como Delivered las entregas de un pedido cuando el cliente confirma la recepción.
/// </summary>
public record MarkDeliveriesDeliveredByOrderCommand(int OrderId);
