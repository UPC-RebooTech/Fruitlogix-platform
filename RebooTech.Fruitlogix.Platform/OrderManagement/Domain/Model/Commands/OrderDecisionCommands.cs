namespace RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;

/// <summary>El productor acepta el pedido: Pending → InPreparation.</summary>
public record AcceptOrderCommand(int OrderId);

/// <summary>El productor rechaza el pedido: Pending o InPreparation → Rejected, con motivo.</summary>
public record RejectOrderCommand(int OrderId, string Reason);
