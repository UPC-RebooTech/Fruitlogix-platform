using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;

public class Order : IAuditableEntity
{
    public int Id { get; private set; }
    public int CommercialClientId { get; private set; }
    public ProducerId? ProducerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DeliveryDate DeliveryDueDate { get; private set; } = null!;
    
    // --- NUEVO: La dirección que viene del frontend ---
    public string DeliveryAddress { get; private set; } = null!; 
    
    // --- ELIMINADOS: FruitType y TotalVolume ya no van aquí ---
    
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    // --- Recepción (US35): null hasta que el cliente confirma ---
    public int? ReceptionRating { get; private set; }
    public string? ReceptionComment { get; private set; }
    public bool? SealAndTemperatureOk { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }

    /// <summary>Lote de cosecha vinculado al preparar el pedido (null hasta que se vincule).</summary>
    public int? BatchId { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Order() { }

    public Order(CreateOrderCommand command)
    {
        CommercialClientId = command.CommercialClientId;
        
        // ¡OJO AQUÍ! El ProducerId nace nulo porque el Cliente no lo elige.
        ProducerId = null; 
        
        DeliveryDueDate = new DeliveryDate(command.DeliveryDueDate);
        DeliveryAddress = command.DeliveryAddress; // Guardamos la dirección
        Notes = command.Notes;
        Status = OrderStatus.Pending;

        if (command.Items is null || command.Items.Count == 0)
            throw new ArgumentException("An order must have at least one item.");

        // Subtotales y total se calculan en el servidor; los que envía el cliente se ignoran.
        foreach (var item in command.Items)
            _items.Add(new OrderItem(item.ProductId, item.ProductName, item.QuantityKg, item.UnitPrice,
                CalculateSubtotal(item.QuantityKg, item.UnitPrice)));
        TotalAmount = _items.Sum(i => i.Subtotal);
    }

    public void AssignProducer(AssignProducerCommand command)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.InPreparation)
            throw new InvalidOperationException("Producer can only be assigned to pending or in-preparation orders.");
        ProducerId = new ProducerId(command.ProducerId);
        if (Status == OrderStatus.Pending)
            Status = OrderStatus.InPreparation;
    }

    public void UpdateDetails(UpdateOrderCommand command)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be edited.");
        
        // También quitamos FruitType y TotalVolume de aquí
        DeliveryDueDate = new DeliveryDate(command.DeliveryDueDate);
        DeliveryAddress = command.DeliveryAddress;
        // TotalAmount no se actualiza: siempre es la suma de los subtotales de los ítems.
        Notes = command.Notes;
    }

    public void MarkInTransit()
    {
        if (Status != OrderStatus.InPreparation)
            throw new InvalidOperationException("Order must be in preparation to mark as in transit.");
        Status = OrderStatus.InTransit;
    }

    public void MarkDelivered()
    {
        if (Status != OrderStatus.InTransit)
            throw new InvalidOperationException("Order must be in transit to mark as delivered.");
        Status = OrderStatus.Delivered;
    }

    public void Cancel(CancelOrderCommand command)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.InTransit)
            throw new InvalidOperationException("Cannot cancel an order that is in transit or delivered.");
        CancellationReason = command.Reason;
        Status = OrderStatus.Cancelled;
    }

    public void Reject(string reason)
    {
        if (Status != OrderStatus.InPreparation)
            throw new InvalidOperationException("Only orders in preparation can be rejected.");
        CancellationReason = reason;
        Status = OrderStatus.Rejected;
    }
    
    /// <summary>Subtotal = cantidad (kg) × precio unitario, redondeado a 2 decimales.</summary>
    public static decimal CalculateSubtotal(double quantityKg, decimal unitPrice) =>
        Math.Round((decimal)quantityKg * unitPrice, 2, MidpointRounding.AwayFromZero);

    public void AssignBatch(int batchId)
    {
        if (batchId <= 0)
            throw new ArgumentException("BatchId must be a positive integer.");
        if (Status is not (OrderStatus.Pending or OrderStatus.InPreparation))
            throw new InvalidOperationException("A batch can only be linked to pending or in-preparation orders.");
        BatchId = batchId;
    }

    /// <summary>
    ///     El despacho de la entrega comenzó: Pending o InPreparation pasan a InTransit.
    ///     En otro estado no cambia nada y devuelve false.
    /// </summary>
    public bool MarkDispatched()
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.InPreparation)) return false;
        Status = OrderStatus.InTransit;
        return true;
    }

    public const int MaxReceptionCommentLength = 500;

    public void ConfirmReception(int? rating, string? comment, bool hasIncidence, bool? sealAndTemperatureOk = null)
    {
        if (Status != OrderStatus.InTransit)
            throw new InvalidOperationException("Order must be in transit to confirm reception.");

        // 0 o null = sin calificar (la app envía 0 cuando el cliente no califica).
        int? normalizedRating = rating is null or 0 ? null : rating;
        if (normalizedRating is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5, or 0/null when not rated.");

        var trimmedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        if (trimmedComment is { Length: > MaxReceptionCommentLength })
            throw new ArgumentException($"Comment cannot exceed {MaxReceptionCommentLength} characters.", nameof(comment));

        Status = hasIncidence ? OrderStatus.DeliveredWithIncidence : OrderStatus.Delivered;
        ReceptionRating = normalizedRating;
        ReceptionComment = trimmedComment;
        SealAndTemperatureOk = sealAndTemperatureOk;
        DeliveredAt = DateTimeOffset.UtcNow;
    }
    
}