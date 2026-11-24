namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

/// <summary>Lectura de telemetría de una entrega.</summary>
/// <param name="Latitude">Latitud GPS (-90 a 90). (0,0) = sin señal.</param>
/// <param name="Longitude">Longitud GPS (-180 a 180).</param>
/// <param name="Temperature">Temperatura en °C.</param>
/// <param name="Humidity">Humedad relativa en %.</param>
public record TrackingLogResource(
    int      Id,
    int      DeliveryId,
    DateTime Timestamp,
    double   Latitude,
    double   Longitude,
    double   Temperature,
    double   Humidity
);