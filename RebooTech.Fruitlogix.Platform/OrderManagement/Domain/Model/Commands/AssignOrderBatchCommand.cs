namespace RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;

/// <summary>Vincula un lote de cosecha (harvest-batches) con el pedido.</summary>
public record AssignOrderBatchCommand(int OrderId, int BatchId);

/// <summary>El despacho de la entrega comenzó: el pedido pasa a InTransit.</summary>
public record MarkOrderDispatchedCommand(int OrderId);
