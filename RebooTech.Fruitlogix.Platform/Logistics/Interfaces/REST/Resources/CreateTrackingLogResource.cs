namespace RebooTech.Fruitlogix.Platform.Logistics.Interfaces.REST.Resources;

/// <summary>Lectura de telemetría que envía el dispositivo IoT o el distribuidor.</summary>
/// <param name="DeliveryId">Entrega a la que pertenece la lectura.</param>
/// <param name="Timestamp">Momento de la lectura (UTC, ISO 8601).</param>
/// <param name="Latitude">Latitud GPS en grados decimales, de -90 a 90. Enviar 0 junto con longitude 0 si no hay señal: la app ignora (0,0).</param>
/// <param name="Longitude">Longitud GPS en grados decimales, de -180 a 180.</param>
/// <param name="Temperature">Temperatura en °C.</param>
/// <param name="Humidity">Humedad relativa en %.</param>
public record CreateTrackingLogResource(
    int      DeliveryId,
    DateTime Timestamp,
    double   Latitude,
    double   Longitude,
    double   Temperature,
    double   Humidity
);