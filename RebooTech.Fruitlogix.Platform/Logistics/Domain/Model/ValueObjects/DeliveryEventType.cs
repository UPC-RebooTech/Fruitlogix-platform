namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;

/// <summary>Tipo de evento en el historial de una entrega.</summary>
public enum DeliveryEventType
{
    /// <summary>La entrega se registró.</summary>
    Created,
    /// <summary>Comenzó el despacho (start-dispatch).</summary>
    Dispatched,
    /// <summary>Se reportó un retraso (report-delay).</summary>
    Delayed,
    /// <summary>Punto de control con nombre en la ruta.</summary>
    Checkpoint,
    /// <summary>Llegó al destino (arrive o geocerca).</summary>
    Arrived,
    /// <summary>El cliente confirmó la recepción.</summary>
    Delivered
}
