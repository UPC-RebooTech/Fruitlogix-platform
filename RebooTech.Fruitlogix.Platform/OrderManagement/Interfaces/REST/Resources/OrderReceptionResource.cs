namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

/// <summary>
///     Datos de la recepción confirmada por el cliente (US35).
///     <c>Rating</c> es null cuando el cliente no calificó.
/// </summary>
public record OrderReceptionResource(
    int OrderId,
    int? Rating,
    string? Comment,
    bool HasIncidence,
    bool? SealAndTemperatureOk,
    DateTimeOffset? ReceivedAt
);
