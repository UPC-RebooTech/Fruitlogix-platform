using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

/// <summary>Etapas de un pedido. status de cada etapa: Completed, Pending o NotApplicable (pedido cancelado o rechazado).</summary>
public enum TraceabilityStageStatus
{
    Completed,
    Pending,
    NotApplicable
}

/// <summary>Una etapa de la trazabilidad (US72), con fecha, responsable y datos propios de la etapa.</summary>
/// <param name="Stage">Harvest, Inspection, Dispatch, Reception o Invoice.</param>
/// <param name="Details">Datos de la etapa en texto (p. ej. batchId, vehiclePlate, rating).</param>
public record TraceabilityStageResource(
    string Stage,
    [property: SwaggerEnumValues(typeof(TraceabilityStageStatus))] string Status,
    DateTimeOffset? OccurredAt,
    string? Responsible,
    IReadOnlyDictionary<string, string?> Details
);

/// <summary>Trazabilidad completa del pedido: cosecha → inspección → despacho → recepción → factura.</summary>
public record OrderTraceabilityResource(
    int OrderId,
    string OrderStatus,
    int? BatchId,
    IReadOnlyList<TraceabilityStageResource> Stages
);
