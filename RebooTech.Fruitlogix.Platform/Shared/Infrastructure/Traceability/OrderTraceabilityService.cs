using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Traceability;

/// <summary>
///     Arma la trazabilidad del pedido (US72) leyendo varios bounded contexts:
///     lote (QualityControl), inspección, entrega (Logistics), recepción (Order) y factura (Payment).
///     Solo lectura; la autorización la hace el controller.
/// </summary>
public interface IOrderTraceabilityService
{
    /// <summary>null si el pedido no existe.</summary>
    Task<OrderTraceabilityResource?> GetAsync(int orderId);
}

public class OrderTraceabilityService(AppDbContext context) : IOrderTraceabilityService
{
    private static string Inv(IFormattable value) => value.ToString(null, CultureInfo.InvariantCulture);

    public async Task<OrderTraceabilityResource?> GetAsync(int orderId)
    {
        var order = await context.Set<Order>().AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return null;

        var closed = order.Status is OrderStatus.Cancelled or OrderStatus.Rejected;
        string Pending() => (closed ? TraceabilityStageStatus.NotApplicable : TraceabilityStageStatus.Pending).ToString();
        var completed = TraceabilityStageStatus.Completed.ToString();

        var stages = new List<TraceabilityStageResource>();

        // 1. Cosecha (lote vinculado con PATCH /orders/{id}/batch)
        var batch = order.BatchId is { } batchId
            ? await context.Set<HarvestBatch>().AsNoTracking().FirstOrDefaultAsync(b => b.Id == batchId)
            : null;
        var producerId = batch?.ProducerId ?? order.ProducerId?.Value;
        var producerName = await ProducerNameAsync(producerId);
        stages.Add(batch is null
            ? new TraceabilityStageResource("Harvest", Pending(), null, producerName, new Dictionary<string, string?>())
            : new TraceabilityStageResource("Harvest", completed,
                new DateTimeOffset(batch.HarvestDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
                producerName,
                new Dictionary<string, string?>
                {
                    ["batchId"] = Inv(batch.Id),
                    ["fruitType"] = batch.FruitType,
                    ["quantityKg"] = Inv(batch.QuantityKg),
                    ["batchStatus"] = batch.Status.ToString()
                }));

        // 2. Inspección de calidad del lote
        var inspection = batch is null
            ? null
            : await context.Set<QualityInspection>().AsNoTracking()
                .Include(q => q.VisualInspection)
                .Include(q => q.TechnicalParameters)
                .Where(q => q.BatchId == batch.Id)
                .OrderByDescending(q => q.Id)
                .FirstOrDefaultAsync();
        stages.Add(inspection is null
            ? new TraceabilityStageResource("Inspection", Pending(), null, null, new Dictionary<string, string?>())
            : new TraceabilityStageResource("Inspection", completed, inspection.CreatedAt,
                inspection.InspectorName ?? await UserNameAsync(inspection.InspectorUserId),
                new Dictionary<string, string?>
                {
                    ["inspectionId"] = Inv(inspection.Id),
                    ["appearanceRating"] = inspection.VisualInspection is null ? null : Inv(inspection.VisualInspection.AppearanceRating),
                    ["wastePercentage"] = inspection.VisualInspection is null ? null : Inv(inspection.VisualInspection.WastePercentage),
                    ["temperatureCelsius"] = inspection.TechnicalParameters is null ? null : Inv(inspection.TechnicalParameters.TemperatureCelsius),
                    ["brixDegrees"] = inspection.TechnicalParameters is null ? null : Inv(inspection.TechnicalParameters.BrixDegrees),
                    ["notes"] = inspection.Notes
                }));

        // 3. Carga y despacho (última entrega del pedido)
        var delivery = await context.Set<Delivery>().AsNoTracking()
            .Where(d => d.OrderId == order.Id)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync();
        var dispatched = delivery is not null
                         && (delivery.DispatchedAt is not null || delivery.CurrentStatus != DeliveryStatus.PendingDispatch);
        stages.Add(delivery is null
            ? new TraceabilityStageResource("Dispatch", Pending(), null, null, new Dictionary<string, string?>())
            : new TraceabilityStageResource("Dispatch", dispatched ? completed : Pending(), delivery.DispatchedAt,
                delivery.Driver.Name,
                new Dictionary<string, string?>
                {
                    ["deliveryId"] = Inv(delivery.Id),
                    ["deliveryStatus"] = delivery.CurrentStatus.ToString(),
                    ["vehiclePlate"] = delivery.Vehicle.Plate,
                    ["vehicleType"] = delivery.Vehicle.Type,
                    ["route"] = $"{delivery.Route.Origin} → {delivery.Route.Destination}",
                    ["distributor"] = await UserNameAsync(delivery.DistributorUserId),
                    ["arrivedAt"] = delivery.ArrivedAt?.ToString("O")
                }));

        // 4. Recepción del cliente
        var received = order.DeliveredAt is not null
                       || order.Status is OrderStatus.Delivered or OrderStatus.DeliveredWithIncidence;
        stages.Add(new TraceabilityStageResource("Reception", received ? completed : Pending(), order.DeliveredAt,
            await ClientNameAsync(order.CommercialClientId),
            received
                ? new Dictionary<string, string?>
                {
                    ["rating"] = order.ReceptionRating is { } rating ? Inv(rating) : null,
                    ["hasIncidence"] = (order.Status == OrderStatus.DeliveredWithIncidence).ToString().ToLowerInvariant(),
                    ["sealAndTemperatureOk"] = order.SealAndTemperatureOk?.ToString().ToLowerInvariant(),
                    ["comment"] = order.ReceptionComment
                }
                : new Dictionary<string, string?>()));

        // 5. Factura
        var invoice = await context.Set<Invoice>().AsNoTracking()
            .Where(i => i.OrderId == order.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();
        stages.Add(invoice is null
            ? new TraceabilityStageResource("Invoice", Pending(), null, null, new Dictionary<string, string?>())
            : new TraceabilityStageResource("Invoice", completed, invoice.IssuedAt,
                await IssuerNameAsync(invoice.IssuedByUserId),
                new Dictionary<string, string?>
                {
                    ["invoiceId"] = Inv(invoice.Id),
                    ["invoiceStatus"] = invoice.Status.ToString(),
                    ["amount"] = Inv(invoice.TotalAmount.Amount),
                    ["currency"] = invoice.TotalAmount.Currency,
                    ["dueDate"] = invoice.DueDate.Value.ToString("yyyy-MM-dd"),
                    ["paidAt"] = invoice.PaidAt?.ToString("O")
                }));

        return new OrderTraceabilityResource(order.Id, order.Status.ToString(), order.BatchId, stages);
    }

    private async Task<string?> UserNameAsync(int? userId) =>
        userId is null
            ? null
            : await context.Set<User>().AsNoTracking()
                .Where(u => u.Id == userId).Select(u => u.Username).FirstOrDefaultAsync();

    private async Task<string?> ProducerNameAsync(int? producerId)
    {
        if (producerId is null) return null;
        var producer = await context.Set<Producer>().AsNoTracking()
            .Where(p => p.Id == producerId).Select(p => new { p.FullName, p.LegalName }).FirstOrDefaultAsync();
        return producer is null ? null : string.IsNullOrWhiteSpace(producer.FullName) ? producer.LegalName : producer.FullName;
    }

    private async Task<string?> ClientNameAsync(int clientUserId)
    {
        var businessName = await context.Set<CommercialClient>().AsNoTracking()
            .Where(c => c.UserId == clientUserId).Select(c => c.BusinessName).FirstOrDefaultAsync();
        return businessName ?? await UserNameAsync(clientUserId);
    }

    /// <summary>Si el emisor es un productor se usa su nombre de perfil; si no, su username.</summary>
    private async Task<string?> IssuerNameAsync(int? userId)
    {
        if (userId is null) return null;
        var user = await context.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;
        if (user.Role == UserRole.Producer)
        {
            var producerId = await context.Set<Producer>().AsNoTracking()
                .Where(p => p.UserId == user.Id).Select(p => (int?)p.Id).FirstOrDefaultAsync();
            return await ProducerNameAsync(producerId) ?? user.Username;
        }
        return user.Username;
    }
}
