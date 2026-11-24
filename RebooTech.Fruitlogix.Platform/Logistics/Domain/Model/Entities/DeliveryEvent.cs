using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;

/// <summary>
///     Evento del historial de una entrega (despacho, retraso, punto de control, llegada, entrega).
///     Las coordenadas son opcionales, pero si vienen deben venir las dos.
/// </summary>
public class DeliveryEvent
{
    public const int MaxNameLength = 100;
    public const int MaxNoteLength = 500;

    public int Id { get; private set; }
    public int DeliveryId { get; private set; }
    public DeliveryEventType Type { get; private set; }
    /// <summary>Nombre del punto de control (obligatorio en Checkpoint).</summary>
    public string? Name { get; private set; }
    public string? Note { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    /// <summary>Usuario IAM que registró el evento (null si fue automático o en datos antiguos).</summary>
    public int? CreatedByUserId { get; private set; }

    protected DeliveryEvent() { }

    public DeliveryEvent(int deliveryId, DeliveryEventType type, int? createdByUserId,
        string? name = null, string? note = null, double? latitude = null, double? longitude = null,
        DateTimeOffset? occurredAt = null)
    {
        if (deliveryId <= 0)
            throw new ArgumentException("DeliveryId must be a positive integer.");

        Name = Trimmed(name, nameof(name), MaxNameLength);
        Note = Trimmed(note, nameof(note), MaxNoteLength);
        if (type == DeliveryEventType.Checkpoint && Name is null)
            throw new ArgumentException("A checkpoint requires a name.", nameof(name));

        if (latitude.HasValue != longitude.HasValue)
            throw new ArgumentException("Latitude and longitude must be sent together.");
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));

        DeliveryId = deliveryId;
        Type = type;
        Latitude = latitude;
        Longitude = longitude;
        OccurredAt = occurredAt ?? DateTimeOffset.UtcNow;
        CreatedByUserId = createdByUserId;
    }

    private static string? Trimmed(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{field} cannot exceed {maxLength} characters.", field);
        return trimmed;
    }
}
