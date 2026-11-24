namespace RebooTech.Fruitlogix.Platform.OrderManagement.Interfaces.REST.Resources;

/// <param name="Rating">Opcional: 1–5. 0 o null = sin calificar.</param>
/// <param name="SealAndTemperatureOk">Opcional: el cliente confirmó sello y temperatura correctos.</param>
public record ConfirmOrderReceptionResource(
    int? Rating,
    string? Comment,
    bool HasIncidence,
    bool? SealAndTemperatureOk = null
);