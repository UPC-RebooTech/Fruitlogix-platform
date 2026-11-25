using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Transform;

public static class OrderResourceFromEntityAssembler
{
    public static OrderResource ToResourceFromEntity(Order order, string? producerName = null,
        string? commercialClientName = null) =>
        new(
            order.Id,
            order.CommercialClientId,
            order.ProducerId?.Value,
            order.Status.ToString(),
            order.DeliveryDueDate.Value,
            order.DeliveryAddress,
            order.TotalAmount,
            order.Notes,
            order.CancellationReason,
            order.Items.Select(i => new OrderItemResource(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.QuantityKg,
                i.UnitPrice,
                i.Subtotal)).ToList(),
            order.CreatedAt,
            order.UpdatedAt,
            order.DeliveredAt,
            order.DeliveredAt,
            ToReceptionResourceFromEntity(order),
            order.BatchId,
            producerName,
            commercialClientName
        );

    /// <summary>null si el pedido aún no tiene recepción confirmada.</summary>
    public static OrderReceptionResource? ToReceptionResourceFromEntity(Order order) =>
        order.DeliveredAt is null
            ? null
            : new OrderReceptionResource(
                order.Id,
                order.ReceptionRating,
                order.ReceptionComment,
                order.Status == OrderStatus.DeliveredWithIncidence,
                order.SealAndTemperatureOk,
                order.DeliveredAt);
}