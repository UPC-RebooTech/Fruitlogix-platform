namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

/// <param name="BatchId">Id del lote en /harvest-batches. Debe pertenecer al productor del pedido.</param>
public record AssignOrderBatchResource(int? BatchId);
